using UnityEngine;
using System;
using System.IO;

namespace Lars.Sound
{
    [RequireComponent(typeof(AudioListener))]
    public class WavRecorder : FilterBase
    {
        #region Fields, Properties, and Inner Classes

        // constants for the wave file header
        private const int HEADER_SIZE = 44;
        private const short BITS_PER_SAMPLE = 16;

        // the audio stream instance
        private MemoryStream outputStream;
        private BinaryWriter outputWriter;

        // should this object be rendering to the output stream?
        private bool isRecording = false;

        // used to manage the length of recording
        private int counter, maxCount;
        private float recDuration;

        string fileName;

        AudioSource audioSource;

        /// The status of a render
        public enum Status
        {
            UNKNOWN,
            SUCCESS,
            FAIL,
            ASYNC
        }

        /// The result of a render.
        public class Result
        {
            public Status State;
            public string Message;

            public Result(Status newState = Status.UNKNOWN, string newMessage = "")
            {
                this.State = newState;
                this.Message = newMessage;
            }
        }
        #endregion

        public WavRecorder()
        {
            this.Clear();
        }


        #region Methods

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            setRecordingDuration(8f);
        }

        void Start()
        {
        }

        /// <summary>
        /// Sets duration of recording and calculates 'maxCount' - the amount of blocks to be stored
        /// </summary>
        /// <param name="dur"></param>
        private void setRecordingDuration(float dur)
        {
            recDuration = dur;

            //Calculate the amount of blocks to be stored to get an 'recDuration' seconds recording
            maxCount = (int)(CHANNELCOUNT * recDuration * SAMPLERATE) / (CHANNELCOUNT * BLOCK_SIZE);
        }

        /// <summary>
        /// Resets the wavrecorder's outputstream & writer
        /// </summary>
        public void Clear()
        {
            this.outputStream = new MemoryStream();
            this.outputWriter = new BinaryWriter(outputStream);

            counter = 0;
        }

        /// <summary>
        /// Write a chunk of data to the output stream.
        /// </summary>
        /// <param name="audioData">interleaved audio data</param>
        public void Write(float[] audioData)
        {
            // Convert numeric audio data to bytes
            for (int i = 0; i < audioData.Length; i++)
            {
                // write the short to the stream
                this.outputWriter.Write((short)(audioData[i] * (float)Int16.MaxValue));
            }
        }

        /// <summary>
        /// write the incoming audio to the output string (Unity function)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channels"></param>
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (this.isRecording)
            {
                // store the data stream
                this.Write(data);

                counter++;
                if (counter >= maxCount)
                {
                    stopRecording();
                }
            }

        }

        /// <summary>
        /// Starts the wav recording
        /// </summary>
        /// <param name="testName">Filename for the recorded wav (in projectfolder/testresult)</param>
        /// <param name="dur"></param>
        public void startRecording(string testName, float dur = 8f)
        {
            if(isRecording)
            {
                Debug.Log("WavRecorder: Already recording!");
                return;
            }
            setRecordingDuration(dur);
            //audioSource.Stop();
            Clear();
            //audioSource.Play();
            isRecording = true;

            fileName = testName;
        }

        /// <summary>
        /// Automatically called when maxCount is reached (all blocks recorded)
        /// </summary>
        private void stopRecording()
        {
            isRecording = false;
            Save("TestResults/" + fileName + ".wav");
        }
        #endregion

        #region File I/O
        public WavRecorder.Result Save(string filename)
        {
            Result result = new WavRecorder.Result();

            if (outputStream.Length > 0)
            {
                // add a header to the file so we can send it to the SoundPlayer
                this.AddHeader();

                // if a filename was passed in
                if (filename.Length > 0)
                {
                    // Save to a file. Print a warning if overwriting a file.
                    if (File.Exists(filename))
                        Debug.LogWarning("Overwriting " + filename + "...");

                    // reset the stream pointer to the beginning of the stream
                    outputStream.Position = 0;

                    // write the stream to a file
                    FileStream fs = File.OpenWrite(filename);

                    this.outputStream.WriteTo(fs);

                    fs.Close();

                    // for debugging only
                    Debug.Log("Finished saving to " + filename + ".");
                }

                result.State = Status.SUCCESS;
            }
            else
            {
                Debug.LogWarning("There is no audio data to save!");

                result.State = Status.FAIL;
                result.Message = "There is no audio data to save!";
            }

            return result;
        }

        /// This generates a simple header for a canonical wave file, 
        /// which is the simplest practical audio file format. It
        /// writes the header and the audio file to a new stream, then
        /// moves the reference to that stream.
        /// 
        /// See this page for details on canonical wave files: 
        /// http://www.lightlink.com/tjweber/StripWav/Canon.html
        private void AddHeader()
        {
            // reset the output stream
            outputStream.Position = 0;

            // calculate the number of samples in the data chunk
            long numberOfSamples = outputStream.Length / (BITS_PER_SAMPLE / 8);

            // create a new MemoryStream that will have both the audio data AND the header
            MemoryStream newOutputStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(newOutputStream);

            writer.Write(0x46464952); // "RIFF" in ASCII

            // write the number of bytes in the entire file
            writer.Write((int)(HEADER_SIZE + (numberOfSamples * BITS_PER_SAMPLE / 8)) - 8);

            writer.Write(0x45564157); // "WAVE" in ASCII
            writer.Write(0x20746d66); // "fmt " in ASCII
            writer.Write(16);

            // write the format tag. 1 = PCM
            writer.Write((short)1);

            // write the number of channels.
            writer.Write((short)CHANNELCOUNT);

            // write the sample rate. The number of audio samples per second
            writer.Write(SAMPLERATE);

            // avg bytes per sec
            writer.Write(SAMPLERATE * CHANNELCOUNT * (BITS_PER_SAMPLE / 8));

            // block align
            writer.Write((short)(CHANNELCOUNT * (BITS_PER_SAMPLE / 8)));

            // 16 bits per sample
            writer.Write(BITS_PER_SAMPLE);

            // "data" in ASCII. Start the data chunk.
            writer.Write(0x61746164);

            // write the number of bytes in the data portion
            writer.Write((int)(numberOfSamples * BITS_PER_SAMPLE / 8));

            // copy over the actual audio data
            this.outputStream.WriteTo(newOutputStream);

            // move the reference to the new stream
            this.outputStream = newOutputStream;
        }
        #endregion
    }

}