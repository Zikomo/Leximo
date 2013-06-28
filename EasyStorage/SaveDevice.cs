using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace EasyStorage
{
	/// <summary>
	/// A base class for an object that maintains a StorageDevice.
	/// </summary>
	/// <remarks>
	/// We implement the three interfaces rather than deriving from GameComponent
	/// just to simplify our constructor and remove the need to pass the Game to
	/// it.
	/// </remarks>
	public abstract class SaveDevice : IGameComponent, IUpdateable, ISaveDevice
	{
		#region Static Strings

		// strings for various message boxes
		private static string promptForCancelledMessage;
		private static string forceCancelledReselectionMessage;
		private static string promptForDisconnectedMessage;
		private static string forceDisconnectedReselectionMessage;
		private static string deviceRequiredTitle;
		private static string deviceOptionalTitle;
		private static readonly string[] deviceOptionalOptions = new string[2];
		private static readonly string[] deviceRequiredOptions = new string[1];

		/// <summary>
		/// Gets or sets the message displayed when the user is asked if they want
		/// to select a storage device after cancelling the storage device selector.
		/// </summary>
		public static string PromptForCancelledMessage
		{
			get { return promptForCancelledMessage; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					promptForCancelledMessage = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the message displayed when the user is told they must
		/// select a storage device after cancelling the storage device selector.
		/// </summary>
		public static string ForceCancelledReselectionMessage
		{
			get { return forceCancelledReselectionMessage; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					forceCancelledReselectionMessage = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the message displayed when the user is asked if they
		/// want to select a new storage device after the storage device becomes
		/// disconnected.
		/// </summary>
		public static string PromptForDisconnectedMessage
		{
			get { return promptForDisconnectedMessage; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					promptForDisconnectedMessage = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the message displayed when the user is told they must
		/// select a new storage device after the storage device becomes disconnected.
		/// </summary>
		public static string ForceDisconnectedReselectionMessage
		{
			get { return forceDisconnectedReselectionMessage; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					forceDisconnectedReselectionMessage = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the title displayed when the user is required to choose
		/// a storage device.
		/// </summary>
		public static string DeviceRequiredTitle
		{
			get { return deviceRequiredTitle; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					deviceRequiredTitle = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the title displayed when the user is asked if they want
		/// to choose a storage device.
		/// </summary>
		public static string DeviceOptionalTitle
		{
			get { return deviceOptionalTitle; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					deviceOptionalTitle = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the text used for the "Ok" button when the user is told
		/// they must select a storage device.
		/// </summary>
		public static string OkOption
		{
			get { return deviceRequiredOptions[0]; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					deviceRequiredOptions[0] = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the text used for the "Yes" button when the user is asked
		/// if they want to select a storage device.
		/// </summary>
		public static string YesOption
		{
			get { return deviceOptionalOptions[0]; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					deviceOptionalOptions[0] = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		/// <summary>
		/// Gets or sets the text used for the "No" button when the user is asked
		/// if they want to select a storage device.
		/// </summary>
		public static string NoOption
		{
			get { return deviceOptionalOptions[1]; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					deviceOptionalOptions[1] = value.Length < 256 ? value : value.Substring(0, 256);
			}
		}

		#endregion

		// the update order for the component and its enabled state
		private int updateOrder;
		private bool enabled = true;

		// was the device connected last frame?
		private bool deviceWasConnected;

		// the current state of the SaveDevice
		private SaveDevicePromptState state = SaveDevicePromptState.None;

		// we store the callbacks as fields to reduce run-time allocation and garbage
		private readonly AsyncCallback storageDeviceSelectorCallback;
		private readonly AsyncCallback forcePromptCallback;
		private readonly AsyncCallback reselectPromptCallback;

		// arguments for our two types of events
		private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();
		private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

		// the actual storage device
		private StorageDevice storageDevice;

		/// <summary>
		/// Gets the name of the StorageContainer used by this SaveDevice.
		/// </summary>
		public string StorageContainerName { get; private set; }

		/// <summary>
		/// Gets whether the SaveDevice has a valid StorageDevice.
		/// </summary>
		public bool HasValidStorageDevice
		{
			get { return storageDevice != null && storageDevice.IsConnected; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the SaveDevice is enabled for use.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (EnabledChanged != null)
						EnabledChanged(this, null);
				}
			}
		}

		/// <summary>
		/// Gets or sets the order in which the SaveDevice is updated
		/// in the game. Components with a lower UpdateOrder are updated
		/// first.
		/// </summary>
		public int UpdateOrder
		{
			get { return updateOrder; }
			set
			{
				if (updateOrder != value)
				{
					updateOrder = value;
					if (UpdateOrderChanged != null)
						UpdateOrderChanged(this, null);
				}
			}
		}

		/// <summary>
		/// Invoked when a StorageDevice is selected.
		/// </summary>
		public event EventHandler DeviceSelected;

		/// <summary>
		/// Invoked when a StorageDevice selector is canceled.
		/// </summary>
		public event EventHandler<SaveDeviceEventArgs> DeviceSelectorCanceled;

		/// <summary>
		/// Invoked when the user closes a prompt to reselect a StorageDevice.
		/// </summary>
		public event EventHandler<SaveDevicePromptEventArgs> DeviceReselectPromptClosed;

		/// <summary>
		/// Invoked when the StorageDevice is disconnected.
		/// </summary>
		public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

		/// <summary>
		/// Fired when the Enabled property has been changed.
		/// </summary>
		public event EventHandler EnabledChanged;

		/// <summary>
		/// Fired when the UpdateOrder property has been changed.
		/// </summary>
		public event EventHandler UpdateOrderChanged;

		static SaveDevice()
		{
			// reset the strings to fill in the defaults
			EasyStorageSettings.ResetSaveDeviceStrings();
		}

		/// <summary>
		/// Creates a new SaveDevice.
		/// </summary>
		/// <param name="storageContainerName">The name to use when opening a StorageContainer.</param>
		protected SaveDevice(string storageContainerName)
		{
			storageDeviceSelectorCallback = StorageDeviceSelectorCallback;
			reselectPromptCallback = ReselectPromptCallback;
			forcePromptCallback = ForcePromptCallback;
			StorageContainerName = storageContainerName;
		}

		/// <summary>
		/// Allows the SaveDevice to initialize itself.
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// Flags the SaveDevice to prompt for a storage device on the next Update.
		/// </summary>
		public void PromptForDevice()
		{
			// we only let the programmer show the selector if the 
			// SaveDevice isn't busy doing something else.
			if (state == SaveDevicePromptState.None)
				state = SaveDevicePromptState.ShowSelector;
		}

		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		/// <returns>True if the save completed without errors, false otherwise.</returns>
		public bool Save(string fileName, FileAction saveAction)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);

			bool success;

			using (var container = storageDevice.OpenContainer(StorageContainerName))
			{
				string path = Path.Combine(container.Path, fileName);
				try
				{
					using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
						saveAction(stream);
					success = true;
				}
				catch
				{
					success = false;
				}
			}

			return success;
		}

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		/// <returns>True if the load completed without error, false otherwise.</returns>
		public bool Load(string fileName, FileAction loadAction)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);

			bool success = false;
			using (var container = storageDevice.OpenContainer(StorageContainerName))
			{
				string path = Path.Combine(container.Path, fileName);
				if (File.Exists(path))
				{
					try
					{
						using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
							loadAction(stream);
						success = true;
					}
					catch
					{
						success = false;
					}
				}
			}

			return success;
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <returns>True if the file either doesn't exist or was deleted succesfully, false if the file exists but failed to be deleted.</returns>
		public bool Delete(string fileName)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);

			using (var container = storageDevice.OpenContainer(StorageContainerName))
			{
				string path = Path.Combine(container.Path, fileName);
				if (File.Exists(path))
				{
					File.Delete(path);
					if (File.Exists(path))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		public string[] GetFiles()
		{
			return GetFiles(null, null);
		}

		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <param name="directory">A subdirectory to search in the SaveDevice.</param>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		public string[] GetFiles(string directory)
		{
			return GetFiles(directory, null);
		}

		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <param name="directory">A subdirectory to search in the SaveDevice.</param>
		/// <param name="pattern">A search pattern to use to find files.</param>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		public string[] GetFiles(string directory, string pattern)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);

			string[] files;
			using (var container = storageDevice.OpenContainer(StorageContainerName))
			{
				string path = container.Path;

				// figure out the full path
				if (!string.IsNullOrEmpty(directory))
					path = Path.Combine(container.Path, directory);

				// get the files
				files = string.IsNullOrEmpty(pattern) ? Directory.GetFiles(path) : Directory.GetFiles(path, pattern);

				// strip out the storage container's path to make these file names compatible with our save/load/delete methods
				for (int i = 0; i < files.Length; i++)
				{
					files[i] = files[i].Substring(container.Path.Length, files[i].Length - container.Path.Length);
				}
			}

			return files;
		}

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		public bool FileExists(string fileName)
		{
			if (!HasValidStorageDevice)
				throw new InvalidOperationException("StorageDevice is not valid.");

			using (var container = storageDevice.OpenContainer(StorageContainerName))
			{
				string path = Path.Combine(container.Path, fileName);
				return File.Exists(path);
			}
		}

		/// <summary>
		/// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
		/// method with the desired parameters, using the given callback.
		/// </summary>
		/// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
		protected abstract void GetStorageDevice(AsyncCallback callback);

		/// <summary>
		/// Prepares the SaveDeviceEventArgs to be used for an event.
		/// </summary>
		/// <param name="args">The event arguments to be configured.</param>
		protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			args.Response = SaveDeviceEventResponse.Prompt;
			args.PlayerToPrompt = PlayerIndex.One;
		}

		/// <summary>
		/// Allows the component to update itself.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public void Update(GameTime gameTime)
		{
			// make sure gamer services are available for all of our Guide methods we use			
			if (!GamerServicesDispatcher.IsInitialized)
				throw new InvalidOperationException(Strings.NeedGamerService);

			bool deviceIsConnected = storageDevice != null && storageDevice.IsConnected;

			if (!deviceIsConnected && deviceWasConnected)
			{
				// if the device was disconnected, fire off the event and handle result
				PrepareEventArgs(eventArgs);

				if (DeviceDisconnected != null)
					DeviceDisconnected(this, eventArgs);

				HandleEventArgResults();
			}
			else if (!deviceIsConnected)
			{
				// we use the try/catch because of the asynchronous nature of the Guide. 
				// the Guide may not be visible when we do our test, but it may open 
				// up after that point and before we've made a call, causing our Guide
				// methods to throw exceptions.
				try
				{
					if (!Guide.IsVisible)
					{
						switch (state)
						{
							// show the normal storage device selector
							case SaveDevicePromptState.ShowSelector:
								state = SaveDevicePromptState.None;
								GetStorageDevice(storageDeviceSelectorCallback);
								break;

							// the user cancelled the device selector, and we've decided to 
							// see if they want another chance to choose a device
							case SaveDevicePromptState.PromptForCanceled:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									deviceOptionalTitle,
									promptForCancelledMessage,
									deviceOptionalOptions,
									0,
									MessageBoxIcon.None,
									reselectPromptCallback,
									null);
								break;

							// the user cancelled the device selector, and we've decided to
							// force them to choose again. this message is simply to inform
							// the user of that.	
							case SaveDevicePromptState.ForceCanceledReselection:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									deviceRequiredTitle,
									forceCancelledReselectionMessage,
									deviceRequiredOptions,
									0,
									MessageBoxIcon.None,
									forcePromptCallback,
									null);
								break;

							// the device has been disconnected, and we've decided to ask
							// the user if they want to choose a new one
							case SaveDevicePromptState.PromptForDisconnected:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									deviceOptionalTitle,
									promptForDisconnectedMessage,
									deviceOptionalOptions,
									0,
									MessageBoxIcon.None,
									reselectPromptCallback,
									null);
								break;

							// the device has been disconnected, and we've decided to force
							// the user to select a new one. this message is simply to inform
							// the user of that.
							case SaveDevicePromptState.ForceDisconnectedReselection:
								Guide.BeginShowMessageBox(
									eventArgs.PlayerToPrompt,
									deviceRequiredTitle,
									forceDisconnectedReselectionMessage,
									deviceRequiredOptions,
									0,
									MessageBoxIcon.None,
									forcePromptCallback,
									null);
								break;

							default:
								break;
						}
					}
				}

				// catch this one type of exception just to be safe
				catch (GuideAlreadyVisibleException) { }
			}

			deviceWasConnected = deviceIsConnected;
		}

		/// <summary>
		/// A callback for the BeginStorageDeviceSelectorPrompt.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void StorageDeviceSelectorCallback(IAsyncResult result)
		{
			//get the storage device
			storageDevice = Guide.EndShowStorageDeviceSelector(result);

			// if we got a valid device, fire off the DeviceSelected event so
			// that the game knows we have a device
			if (storageDevice != null && storageDevice.IsConnected)
			{
				if (DeviceSelected != null)
					DeviceSelected(this, null);
			}

			// if we don't have a valid device
			else
			{
				// prepare our event arguments for use
				PrepareEventArgs(eventArgs);

				// let the game know the device selector was cancelled so it
				// can tell us how to handle this
				if (DeviceSelectorCanceled != null)
					DeviceSelectorCanceled(this, eventArgs);

				// handle the result of the event
				HandleEventArgResults();
			}
		}

		/// <summary>
		/// A callback for either of the message boxes telling users they
		/// have to choose a storage device, either from cancelling the
		/// device selector or disconnecting the device.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void ForcePromptCallback(IAsyncResult result)
		{
			// just end the message and instruct the SaveDevice to show the selector
			Guide.EndShowMessageBox(result);
			state = SaveDevicePromptState.ShowSelector;
		}

		/// <summary>
		/// A callback for either of the message boxes asking the user
		/// to select a new device, either from cancelling the device
		/// seledctor or disconnecting the device.
		/// </summary>
		/// <param name="result">The result of the prompt.</param>
		private void ReselectPromptCallback(IAsyncResult result)
		{
			int? choice = Guide.EndShowMessageBox(result);

			// get the device if the user chose the first option
			state = choice.HasValue && choice.Value == 0 ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None;

			// fire an event for the game to know the result of the prompt
			promptEventArgs.ShowDeviceSelector = state == SaveDevicePromptState.ShowSelector;
			if (DeviceReselectPromptClosed != null)
				DeviceReselectPromptClosed(this, promptEventArgs);
		}

		/// <summary>
		/// Handles reading from the eventArgs to determine what action to take.
		/// </summary>
		private void HandleEventArgResults()
		{
			// clear the Device reference
			storageDevice = null;

			// determine the next action...
			switch (eventArgs.Response)
			{
				// will have the manager prompt the user with the option of reselecting the storage device
				case SaveDeviceEventResponse.Prompt:
					state = deviceWasConnected
						? SaveDevicePromptState.PromptForDisconnected
						: SaveDevicePromptState.PromptForCanceled;
					break;

				// will have the manager prompt the user that the device must be selected
				case SaveDeviceEventResponse.Force:
					state = deviceWasConnected
						? SaveDevicePromptState.ForceDisconnectedReselection
						: SaveDevicePromptState.ForceCanceledReselection;
					break;

				// will have the manager do nothing
				default:
					state = SaveDevicePromptState.None;
					break;
			}
		}
	}
}