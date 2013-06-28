using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace EasyStorage
{
	/// <summary>
	/// A SaveDevice used for saving player-specific data.
	/// </summary>
	public sealed class PlayerSaveDevice : SaveDevice
	{
		/// <summary>
		/// Gets the PlayerIndex of the player for which the data will be saved.
		/// </summary>
		public PlayerIndex Player { get; private set; }

		/// <summary>
		/// Creates a new PlayerSaveDevice for a given player.
		/// </summary>
		/// <param name="storageContainerName">The name to use when opening a StorageContainer.</param>
		/// <param name="player">The player for which the data will be saved.</param>
		public PlayerSaveDevice(string storageContainerName, PlayerIndex player)
			: base(storageContainerName)
		{
			Player = player;
		}

		/// <summary>
		/// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
		/// method with the desired parameters, using the given callback.
		/// </summary>
		/// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
		protected override void GetStorageDevice(AsyncCallback callback)
		{
			Guide.BeginShowStorageDeviceSelector(Player, callback, null);
		}

		/// <summary>
		/// Prepares the SaveDeviceEventArgs to be used for an event.
		/// </summary>
		/// <param name="args">The event arguments to be configured.</param>
		protected override void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			// the base implementation sets some aspects of the arguments,
			// so we let it do that first
			base.PrepareEventArgs(args);

			// we then default the player to prompt to be the player that
			// owns this storage device. we assume the game will leave this
			// untouched so that the correct player is prompted, but we also
			// allow the game to change it if there's a reason to.
			args.PlayerToPrompt = Player;
		}
	}
}