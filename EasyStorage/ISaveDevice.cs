using System.IO;

namespace EasyStorage
{
	/// <summary>
	/// A method for loading or saving a file.
	/// </summary>
	/// <param name="stream">A Stream to use for accessing the file data.</param>
	public delegate void FileAction(Stream stream);

	/// <summary>
	/// Defines the interface for an object that can perform file operations.
	/// </summary>
	public interface ISaveDevice
	{
		/// <summary>
		/// Saves a file.
		/// </summary>
		/// <param name="fileName">The file to save.</param>
		/// <param name="saveAction">The save action to perform.</param>
		/// <returns>True if the save completed without errors, false otherwise.</returns>
		bool Save(string fileName, FileAction saveAction);

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="fileName">The file to load.</param>
		/// <param name="loadAction">The load action to perform.</param>
		/// <returns>True if the load completed without error, false otherwise.</returns>
		bool Load(string fileName, FileAction loadAction);

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <returns>True if the file either doesn't exist or was deleted succesfully, false if the file exists but failed to be deleted.</returns>
		bool Delete(string fileName);

		/// <summary>
		/// Determines if a given file exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		bool FileExists(string fileName);
		
		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		string[] GetFiles();
        
		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <param name="directory">A subdirectory to search in the SaveDevice.</param>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		string[] GetFiles(string directory);
        
		/// <summary>
		/// Gets an array of all files available in the SaveDevice.
		/// </summary>
		/// <param name="directory">A subdirectory to search in the SaveDevice.</param>
		/// <param name="pattern">A search pattern to use to find files.</param>
		/// <returns>An array of file names of the files in the SaveDevice.</returns>
		string[] GetFiles(string directory, string pattern);
	}
}
