using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyStorage;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using KiloWatt.Runtime.Support;
using Microsoft.Xna.Framework;

namespace IETGames.Shorewood.Storage
{
    public class StorageHandler
    {
        ISaveDevice saveDevice;
        bool useStorage = true;
        public const string settingsFileName = "Leximo.Settings.xml";
        public const string highScoresFileName = "Leximo.High.Scores.xml";
        public const string highScoresGamerPictures = "Leximo.Pictures.bin";
        public SharedSaveDevice sharedSaveDevice;
        private readonly XmlSerializer highScoresSerializer = new XmlSerializer(typeof(HighScores));
        private readonly XmlSerializer settingsSerializer = new XmlSerializer(typeof(Settings));
        public StorageHandler()
        {
            EasyStorageSettings.SetSupportedLanguages(EasyStorage.Language.English);
            HighScores = new HighScores();
            Settings = new Settings();

#if WINDOWS
            saveDevice = new PCSaveDevice("Leximo");
            useStorage = true;
            GetHighScoresFromDevice();
            //sharedSaveDevice = saveDevice;
#else
			// add the GamerServicesComponent
			

			// create and add our SaveDevice
			sharedSaveDevice = new SharedSaveDevice("Leximo");
			//Components.Add(sharedSaveDevice);

			// hook an event for when the device is selected
			sharedSaveDevice.DeviceSelected += (s, e) => GetHighScoresFromDevice();
			
			// hook two event handlers to force the user to choose a new device if they cancel the
			// device selector or if they disconnect the storage device after selecting it
            sharedSaveDevice.DeviceSelectorCanceled += new EventHandler<SaveDeviceEventArgs>(sharedSaveDevice_DeviceSelectorCanceled);//(s, e) => e.Response = SaveDeviceEventResponse.Prompt;
            sharedSaveDevice.DeviceDisconnected += new EventHandler<SaveDeviceEventArgs>(sharedSaveDevice_DeviceDisconnected); //(s, e) => e.Response = SaveDeviceEventResponse.Prompt;

			// prompt for a device on the next Update
			

			// make sure we hold on to the device
			saveDevice = sharedSaveDevice;
#endif
        }
        void sharedSaveDevice_DeviceSelectorCanceled(object sender, SaveDeviceEventArgs e)
        {
            e.Response = SaveDeviceEventResponse.Prompt;
            e.PlayerToPrompt = Shorewood.mainPlayer;
            useStorage = false;
            HighScoresLoaded = true;
        }

        void sharedSaveDevice_DeviceDisconnected(object sender, SaveDeviceEventArgs e)
        {
            e.Response = SaveDeviceEventResponse.Prompt;
            e.PlayerToPrompt = Shorewood.mainPlayer;
            useStorage = false;
            
        }

        public void Reset()
        {
            HighScoresLoaded = false;
            StorageDevicePrompted = false;
        }

        public void PromptForStorageDevice()
        {
#if XBOX

            sharedSaveDevice.PromptForDevice();            
            
#endif
            StorageDevicePrompted = true;              
        }

        public Settings Settings
        {
            get;
            private set;
        }

        public HighScores HighScores
        {
            get;
            private set;
        }

        public bool HighScoresLoaded
        {
            get;
            private set;
        }

        public bool StorageDevicePrompted
        {
            get;
            private set;
        }
        
        public void SaveHighScores(object nothing)
        {
            Shorewood.IsStorageActive = true;
            try
            {
                if (useStorage&&!Shorewood.IsTrial)
                {
                    if (!saveDevice.Save(highScoresFileName, SerializeHighScores)||!saveDevice.Save(highScoresGamerPictures,SerializeGamerPictures))
                    {
                        //do something
                    }
                }
            }
            catch
            {
                //add save error
            }
            finally
            {
                Shorewood.IsStorageActive = false;
            }
        }

        public void LoadHighScores(object nothing)
        {            
            Shorewood.IsStorageActive = true;
            try
            {
                if (useStorage&!Shorewood.IsTrial)
                {
                    if (saveDevice.FileExists(highScoresFileName)&&saveDevice.FileExists(highScoresGamerPictures))
                    {
                            if (saveDevice.Load(highScoresFileName, DeserializeHighScores) && saveDevice.Load(highScoresGamerPictures,DeserializeGamerPictures))
                            {
                                LoadSettings(null);
                                return;
                            }
                        }
                    }
                }
            catch
            {
                // add load error
            }
            finally
            {
                Shorewood.IsStorageActive = false;
            }

            HighScores.Populate();            
            HighScoresLoaded = true;
        }

        protected void SaveSettings(object nothing)
        {
            Shorewood.IsStorageActive = true;
            try
            {
                if (useStorage)
                {
                    if (!saveDevice.Save(settingsFileName, SerializeSettings))
                    {
                        //do something
                    }
                }
            }
            catch
            {
                //add save error
            }
            finally
            {
                Shorewood.IsStorageActive = false;
            }
        }

        protected void LoadSettings(object nothing)
        {            
            Shorewood.IsStorageActive = true;
            try
            {
                if (useStorage)
                {
                    if (saveDevice.FileExists(settingsFileName))
                    {
                        if (saveDevice.Load(settingsFileName, DeserializeSettings))
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // add load error
            }
            finally
            {
                Shorewood.IsStorageActive = false;
            }
            Settings = new Settings();
        }

        public void SerializeSettings(Stream stream)
        {
            settingsSerializer.Serialize(stream, Settings);
        }

        public void DeserializeSettings(Stream stream)
        {
            Settings = settingsSerializer.Deserialize((Stream)stream) as Settings;
        }

        public void SerializeHighScores(Stream stream)
        {                      
            highScoresSerializer.Serialize(stream, HighScores);
        }

        public void DeserializeHighScores(Stream stream)
        {
            HighScores = highScoresSerializer.Deserialize((Stream)stream) as HighScores;
            HighScoresLoaded = true;
            HighScores.Load();
        }

        public void SerializeGamerPictures(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(HighScores.Gamers.Count);
                foreach (var gamer in HighScores.Gamers)
                {
                    GamerEntry.WriteEntry(writer, gamer);
                }
            }
        }

        public void DeserializeGamerPictures(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int count = reader.ReadInt32();
                HighScores.Gamers.Clear();
                for (int i = 0; i < count; i++)
                {
                    HighScores.Gamers.Add(GamerEntry.ReadEntry(reader));
                }
            }
        }

        public void GetSettingsFromDevice()
        {
            Shorewood.threadPool.AddTask(LoadSettings, null, null, null);
        }

        public void SaveSettingsToDevice()
        {
            Shorewood.threadPool.AddTask(SaveSettings, null, null, null);
        }

        public void GetHighScoresFromDevice()
        {
            //ThreadPool.QueueUserWorkItem(loadThread);
            Shorewood.threadPool.AddTask(LoadHighScores, null, null, null);
        }

        public void SaveHighScoresToDevice()
        {
            //ThreadPool.QueueUserWorkItem(saveThread);
            Shorewood.threadPool.AddTask(SaveHighScores, null, null, null);
        }
    }
}