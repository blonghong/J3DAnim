// ------------------------------------------
// J3DAnim
// Converts Maya's ASCII animation format to Nintendo's J3D animation format
// 3/23/17, from MasterF0x
// --
// Now, if you want to take a look at this messy code, have at it. You won't be very happy :}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace J3DAnim
{

    public partial class Form1 : Form
    {
        public class ANIMHeader
        {
            public string version;
            public string mayaVersion;
            public string startTime;
            public string endTime;
        }

        public class ANIM
        {
            // to-do
            public string pad = "This is padding data to align.  ";
            public Int16 scaleCount = 1;
            public Int16 rotCount = 1;
            public Int16 transCount = 1;
            public Int32 scalesOff;
            public Int32 rotsOff;
            public Int32 transOff;

            public List<int> kfTransX = new List<int>();
            public List<int> kfTransY = new List<int>();
            public List<int> kfTransZ = new List<int>();

            public List<bool> setPosTransX = new List<bool>();
            public List<bool> setPosTransY = new List<bool>();
            public List<bool> setPosTransZ = new List<bool>();

            public List<int> kfRotX = new List<int>();
            public List<int> kfRotY = new List<int>();
            public List<int> kfRotZ = new List<int>();

            public List<bool> setRotX = new List<bool>();
            public List<bool> setRotY = new List<bool>();
            public List<bool> setRotZ = new List<bool>();

            public List<int> kfScaleX = new List<int>();
            public List<int> kfScaleY = new List<int>();
            public List<int> kfScaleZ = new List<int>();

            public List<bool> setScaleX = new List<bool>();
            public List<bool> setScaleY = new List<bool>();
            public List<bool> setScaleZ = new List<bool>();

            public List<int> timesTransX = new List<int>();
            public List<int> timesTransY = new List<int>();
            public List<int> timesTransZ = new List<int>();

            public List<int> timesRotX = new List<int>();
            public List<int> timesRotY = new List<int>();
            public List<int> timesRotZ = new List<int>();

            public List<int> timesScaleX = new List<int>();
            public List<int> timesScaleY = new List<int>();
            public List<int> timesScaleZ = new List<int>();

            public List<float> tangentsTransX = new List<float>();
            public List<float> tangentsTransY = new List<float>();
            public List<float> tangentsTransZ = new List<float>();

            public List<float> tangentsRotX = new List<float>();
            public List<float> tangentsRotY = new List<float>();
            public List<float> tangentsRotZ = new List<float>();

            public List<float> tangentsScaleX = new List<float>();
            public List<float> tangentsScaleY = new List<float>();
            public List<float> tangentsScaleZ = new List<float>();

            public List<float> transX = new List<float>();
            public List<float> transY = new List<float>();
            public List<float> transZ = new List<float>();

            public List<int> rotX = new List<int>();
            public List<int> rotY = new List<int>();
            public List<int> rotZ = new List<int>();

            public List<float> scaleX = new List<float>();
            public List<float> scaleY = new List<float>();
            public List<float> scaleZ = new List<float>();

            public List<float> transTotal = new List<float>();
            public List<int> timesTransTotal = new List<int>();

            public List<int> rotsTotal = new List<int>();
            public List<int> timesRotsTotal = new List<int>();

            public List<float> scalesTotal = new List<float>();
            public List<int> timesScalesTotal = new List<int>();
        }

        public class JNT1
        {
            public int header;
            public int sectionSize;
            public int joints;
        }

        public ANIMHeader animHead = new ANIMHeader();
        public JNT1 jnt1 = new JNT1();
        public ANIM anim = new ANIM();

        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile.ShowDialog();
            StreamReader sr = new StreamReader(openFile.FileName, true);
            animHead.version = sr.ReadLine();
            animHead.mayaVersion = sr.ReadLine();
            sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
            animHead.startTime = sr.ReadLine();
            animHead.endTime = sr.ReadLine();

            char[] splitter = { ';', ' ' };
            String[] animVer = animHead.version.Split(splitter);
            String[] mayaVer = animHead.mayaVersion.Split(splitter);
            String[] startT = animHead.startTime.Split(splitter);
            String[] endT = animHead.endTime.Split(splitter);

            textBox1.Text = animVer[1];
            textBox2.Text = mayaVer[1] + "   " + mayaVer[2];
            textBox3.Text = startT[1];
            textBox4.Text = endT[1];

            animHead.endTime = endT[1];

            sr.Close();

            openBMD.ShowDialog();
            EndianBinaryReader br = new EndianBinaryReader(File.Open(openBMD.FileName, FileMode.Open));
            br.BaseStream.Seek(0, 0);
            br.ReadUInt32(); br.ReadUInt32();
            UInt32 bmdSize = br.ReadUInt32();

            // Read the BMD and find the JNT1 chunck
            for (int i = 0; i < bmdSize - 0x32; i+=4)
            {
                UInt32 chunkID = br.ReadUInt32();
                if (chunkID == 0x4A4E5431)
                {
                    //MessageBox.Show("JNT1 detected");
                    jnt1.sectionSize = (int)br.ReadUInt32();
                    jnt1.joints = (int)br.ReadUInt16();
                }
            }

            br.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.FileStream fs = System.IO.File.Create(openFile.FileName + ".bck");
            fs.Close();
            EndianBinaryWriter bw = new EndianBinaryWriter(File.Open(openFile.FileName + ".bck", FileMode.Open));

            FileInfo bck = new FileInfo(openFile.FileName + ".bck");

            // --
            // J3D1bck1 section
            bw.Write(0x4A33443162636B31); // J3D1bck1
            bw.Write((Int32)bck.Length); // size
            bw.Write(0x00000001); // sections
            bw.Write(0xFFFFFFFFFFFFFFFF); // --v
            bw.Write(0xFFFFFFFFFFFFFFFF); // padding
            //--
            // J3D1bck1 parsing ends here

            // --
            //ANK1 section
            bw.Write(0x414E4B31); // ANK1
            bw.Write((Int32)bck.Length - 0x20); // size
            if (radioButton1.Checked) bw.BaseStream.WriteByte(0); // play once
            if (radioButton2.Checked) bw.BaseStream.WriteByte(1); // unknown
            if (radioButton3.Checked) bw.BaseStream.WriteByte(2); // loop
            if (radioButton4.Checked) bw.BaseStream.WriteByte(3); // mirror
            if (radioButton5.Checked) bw.BaseStream.WriteByte(4); // ping pong loop
            bw.BaseStream.WriteByte(1); // angle multiplier
            float t = float.Parse(animHead.endTime);
            bw.Write(Convert.ToInt16(t));
            int joints = jnt1.joints;
            bw.Write((UInt16)joints);

            bw.Write(anim.scaleCount); // scale count
            bw.Write(anim.rotCount); // rotation count
            bw.Write(anim.transCount); // translation count
            bw.Write((Int32)64); // joint off
            bw.Write(anim.scalesOff);
            bw.Write(anim.rotsOff);
            bw.Write(anim.transOff);

            insertPadding(bw, 32, true);

            int pos = (int)bw.BaseStream.Position;
            bw.Close();
            parseAnimationTable(pos);
        }

        void parseAnimationTable(int pos)
        {
            char[] splitL = { ' ', ';' };
            string curSubString = "";
            string[] curl = new string[44];
            int localCount = 0;

            StreamReader sr = new StreamReader(openFile.FileName, true);
            sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
            sr.ReadLine(); sr.ReadLine(); sr.ReadLine();

            // .anim parser - Grabs all the values and keyframes needed to make the animation --------------------------------------------------

            for (;;)
            {
                string currentLine = sr.ReadLine();
                if (currentLine == null)
                {
                    sr.Close();
                    writeKeyframes(pos);
                    return;
                }
                curl = currentLine.Split(splitL);
                switch(curl[1])
                {   
                    // TRANSLATION ------------------------------------------------------------------------
                    case "translate.translateX":

                       for (int j = 0; j < 7; j++) sr.ReadLine();

                       while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesTransX.Add(Convert.ToInt32(tempFloat));
                                anim.timesTransTotal.Add(Convert.ToInt32(tempFloat));
                                anim.transX.Add(float.Parse(curindex[5]));
                                anim.transTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsTransX.Add(0.0f);
                                localCount++;
                                //MessageBox.Show(curindex[curindex.Length - 3]);
                            }
                        }

                        anim.kfTransX.Add(localCount);
                        anim.setPosTransX.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "translate.translateY":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesTransY.Add(Convert.ToInt32(tempFloat));
                                anim.timesTransTotal.Add(Convert.ToInt32(tempFloat));
                                anim.transY.Add(float.Parse(curindex[5]));
                                anim.transTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsTransY.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfTransY.Add(localCount);
                        anim.setPosTransY.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "translate.translateZ":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesTransZ.Add(Convert.ToInt32(tempFloat));
                                anim.timesTransTotal.Add(Convert.ToInt32(tempFloat));
                                anim.transZ.Add(float.Parse(curindex[5]));
                                anim.transTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsTransZ.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfTransZ.Add(localCount);
                        anim.setPosTransZ.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    // ------------------------------------------------------------------------------
                    // ROTATIONS

                    case "rotate.rotateX":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesRotX.Add(Convert.ToInt16(tempFloat));
                                anim.timesRotsTotal.Add(Convert.ToInt32(tempFloat));
                                float temp1 = float.Parse(curindex[5]);
                                int temp2 = Convert.ToInt16(temp1);
                                anim.rotX.Add(temp2);
                                anim.rotsTotal.Add((Int16)temp2 * 90);
                                anim.tangentsRotX.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfRotX.Add(localCount);
                        anim.setRotX.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "rotate.rotateY":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesRotY.Add(Convert.ToInt16(tempFloat));
                                anim.timesRotsTotal.Add(Convert.ToInt32(tempFloat));
                                float temp1 = float.Parse(curindex[5]);
                                int temp2 = Convert.ToInt16(temp1);
                                anim.rotY.Add(temp2);
                                anim.rotsTotal.Add((Int16)temp2 * 90);
                                anim.tangentsRotY.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfRotY.Add(localCount);
                        anim.setRotY.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "rotate.rotateZ":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesRotZ.Add(Convert.ToInt16(tempFloat));
                                anim.timesRotsTotal.Add(Convert.ToInt32(tempFloat));
                                float temp1 = float.Parse(curindex[5]);
                                int temp2 = Convert.ToInt16(temp1);
                                anim.rotZ.Add(temp2);
                                anim.rotsTotal.Add((Int16)temp2 * 90);
                                anim.tangentsRotZ.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfRotZ.Add(localCount);
                        anim.setRotZ.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                        // --------------------------------------------------------------------
                        // SCALES 

                    case "scale.scaleX":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesScaleX.Add(Convert.ToInt32(tempFloat));
                                anim.timesScalesTotal.Add(Convert.ToInt32(tempFloat));
                                anim.scaleX.Add(float.Parse(curindex[5]));
                                anim.scalesTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsScaleX.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfScaleX.Add(localCount);
                        anim.setScaleX.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "scale.scaleY":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesScaleY.Add(Convert.ToInt32(tempFloat));
                                anim.timesScalesTotal.Add(Convert.ToInt32(tempFloat));
                                anim.scaleY.Add(float.Parse(curindex[5]));
                                anim.scalesTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsScaleY.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfScaleY.Add(localCount);
                        anim.setScaleY.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "scale.scaleZ":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                string[] curindex = curSubString.Split(splitL);
                                float tempFloat = float.Parse(curindex[4]);
                                anim.timesScaleZ.Add(Convert.ToInt32(tempFloat));
                                anim.timesScalesTotal.Add(Convert.ToInt32(tempFloat));
                                anim.scaleZ.Add(float.Parse(curindex[5]));
                                anim.scalesTotal.Add(float.Parse(curindex[5]));
                                anim.tangentsScaleZ.Add(0.0f);
                                localCount++;
                            }
                        }

                        anim.kfScaleZ.Add(localCount);
                        anim.setScaleZ.Add(true);
                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                        // ----------------------------------------------------------------------
                        // Other junk :1

                    case "visibility":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                continue;
                            }
                        }

                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;

                    case "MaxHandle":

                        for (int j = 0; j < 7; j++) sr.ReadLine();

                        while (curSubString != "  }")
                        {
                            curSubString = sr.ReadLine();
                            if (curSubString != "  }")
                            {
                                continue;
                            }
                        }

                        localCount = 0;
                        sr.ReadLine();
                        curSubString = "";
                        break;
                }
            }

            // -------------------------------------------------

            writeKeyframes(pos);
            sr.Close();
        }

        void writeKeyframes(int pos)
        {
            initArrays();

            EndianBinaryWriter bw = new EndianBinaryWriter(File.Open(openFile.FileName + ".bck", FileMode.Open));
            bw.BaseStream.Seek(pos, 0);

            int floatIndex = 1;
            int rotIndex = 1;
            int scaleIndex = 1;

            // Keyframes are.. weird
            // This was a pain to do, but I'm just glad that it works :}

            for (int i = 0; i < jnt1.joints; i++)
            {
                // X-AXIS

                bw.Write((Int16)anim.kfScaleX[i]);
                if (anim.setScaleX[i] == false) bw.Write((Int16)0);
                if (anim.setScaleX[i])
                {
                    bw.Write((Int16)scaleIndex);
                    scaleIndex += (anim.kfScaleX[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfRotX[i]);
                if (anim.setRotX[i] == false) bw.Write((Int16)0);
                if (anim.setRotX[i])
                {
                    bw.Write((Int16)rotIndex);
                    rotIndex += (anim.kfRotX[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfTransX[i]);
                if (anim.setPosTransX[i] == false) bw.Write((Int16)0);
                if (anim.setPosTransX[i])
                {
                    bw.Write((Int16)floatIndex);
                    floatIndex += (anim.kfTransX[i] * 3);
                }
                bw.Write((Int16)0);

                // --------------------------------------------------
                // Y-AXIS
                bw.Write((Int16)anim.kfScaleY[i]);
                if (anim.setScaleY[i] == false) bw.Write((Int16)0);
                if (anim.setScaleY[i])
                {
                    bw.Write((Int16)scaleIndex);
                    scaleIndex += (anim.kfScaleY[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfRotY[i]);
                if (anim.setRotY[i] == false) bw.Write((Int16)0);
                if (anim.setRotY[i])
                {
                    bw.Write((Int16)rotIndex);
                    rotIndex += (anim.kfRotY[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfTransY[i]);
                if (anim.setPosTransY[i] == false) bw.Write((Int16)0);
                if (anim.setPosTransY[i])
                {
                    bw.Write((Int16)floatIndex);
                    floatIndex += (anim.kfTransY[i] * 3);
                }
                bw.Write((Int16)0);

                // -----------------------------------------------------
                // Z-AXIS
                bw.Write((Int16)anim.kfScaleZ[i]);
                if (anim.setScaleZ[i] == false) bw.Write((Int16)0);
                if (anim.setScaleZ[i])
                {
                    bw.Write((Int16)scaleIndex);
                    scaleIndex += (anim.kfScaleZ[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfRotZ[i]);
                if (anim.setRotZ[i] == false) bw.Write((Int16)0);
                if (anim.setRotZ[i])
                {
                    bw.Write((Int16)rotIndex);
                    rotIndex += (anim.kfRotZ[i] * 3);
                }
                bw.Write((Int16)0);

                bw.Write((Int16)anim.kfTransZ[i]);
                if (anim.setPosTransZ[i] == false) bw.Write((Int16)0);
                if (anim.setPosTransZ[i])
                {
                    bw.Write((Int16)floatIndex);
                    floatIndex += (anim.kfTransZ[i] * 3);
                }
                bw.Write((Int16)0);
            }

            insertPadding(bw, 32, true);
            pos = (int)bw.BaseStream.Position;
            anim.scalesOff = pos - 32;
            writeScales(pos, bw);
        }

        void writeScales(int pos, EndianBinaryWriter bw)
        {

            bw.BaseStream.Seek(pos, 0);
            bw.Write((float)1.0f);
            for (int i = 0; i < anim.scalesTotal.Count; i++)
            {
                bw.Write((float)anim.timesScalesTotal[i]);
                anim.scaleCount++;
                bw.Write((float)anim.scalesTotal[i]);
                anim.scaleCount++;
                bw.Write((Int32)0);
                anim.scaleCount++;
            }

            insertPadding(bw, 32, true);
            pos = (int)bw.BaseStream.Position;
            anim.rotsOff = pos - 32;
            writeRots(pos, bw);
        }

        void writeRots(int pos, EndianBinaryWriter bw)
        {
            bw.BaseStream.Seek(pos, 0);
            bw.Write((Int16)0.0f);
            for (int i = 0; i < anim.rotsTotal.Count; i++)
            {
                bw.Write((Int16)anim.timesRotsTotal[i]);
                anim.rotCount++;
                bw.Write((Int16)anim.rotsTotal[i]);
                anim.rotCount++;
                bw.Write((Int16)0);
                anim.rotCount++;
            }

            insertPadding(bw, 32, true);
            pos = (int)bw.BaseStream.Position;
            anim.transOff = pos - 32;
            writeTrans(pos, bw);
        }

        void writeTrans(int pos, EndianBinaryWriter bw)
        {
            bw.BaseStream.Seek(pos, 0);
            bw.Write((float)0.0f);
            for (int i = 0; i < anim.transTotal.Count; i++)
            {
                bw.Write((float)anim.timesTransTotal[i]);
                anim.transCount++;
                bw.Write((float)anim.transTotal[i]);
                anim.transCount++;
                bw.Write((Int32)0);
                anim.transCount++;
            }

            insertPadding(bw, 32, true);
            writeOffs(bw);

            MessageBox.Show("Export successful.", "J3DAnim",
            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }


        // This function was made to keep the arrays filled and not go out of index
        // It's also for making keyframes more accurate
        void initArrays()
        {
            for (int i = 0; i < 99; i++)
            {
                anim.kfScaleX.Add(1);
                anim.setScaleX.Add(false);
                anim.kfScaleY.Add(1);
                anim.setScaleY.Add(false);
                anim.kfScaleZ.Add(1);
                anim.setScaleZ.Add(false);

                anim.kfRotX.Add(1);
                anim.setRotX.Add(false);
                anim.kfRotY.Add(1);
                anim.setRotY.Add(false);
                anim.kfRotZ.Add(1);
                anim.setRotZ.Add(false);

                anim.kfTransX.Add(1);
                anim.setPosTransX.Add(false);
                anim.kfTransY.Add(1);
                anim.setPosTransY.Add(false);
                anim.kfTransZ.Add(1);
                anim.setPosTransZ.Add(false);
            }
        }


        // Thank you for this, Gamma and LordNed
        private void insertPadding(EndianBinaryWriter writer, int padValue, bool usePaddingString)
        {
            // Pad up to a 32 byte alignment
            // Formula: (x + (n-1)) & ~(n-1)
            long nextAligned = (writer.BaseStream.Length + (padValue - 1)) & ~(padValue - 1);

            long delta = nextAligned - writer.BaseStream.Length;
            writer.BaseStream.Position = writer.BaseStream.Length;
            char[] padding = anim.pad.ToCharArray();
            for (int i = 0; i < delta; i++)
            {
                if (usePaddingString)
                    writer.BaseStream.WriteByte((byte)padding[i]);
                else
                    writer.Write((byte)0);
            }
        }

        void writeOffs(EndianBinaryWriter bw)
        {
            bw.BaseStream.Seek(0x2C, 0);
            bw.Write((Int16)jnt1.joints);
            bw.Write((Int16)anim.scaleCount);
            bw.Write((Int16)anim.rotCount);
            int t = anim.transCount;
            bw.Write(((Int16)t));
            bw.Write((Int32)0x40);
            bw.Write((Int32)anim.scalesOff);
            bw.Write((Int32)anim.rotsOff);
            bw.Write((Int32)anim.transOff);

            bw.Close();
        }

        // Not used
        private double RadianToDegree(float angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
