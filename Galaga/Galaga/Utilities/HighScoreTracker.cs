using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Galaga.Systems;

namespace Galaga.Utilities;

public class HighScoreTracker
{
     public List<int> HighScores = new();
     public int CurrentGameScore;
     private bool _loading;
     private bool _saving;
     private static HighScoreTracker _tracker;
     private GameStatsSystem _gameStatsSystem;
     

     public static HighScoreTracker GetTracker()
     {
         return _tracker ??= new HighScoreTracker();
     }
     
     private HighScoreTracker()
     {
         LoadScores();
         _gameStatsSystem = GameStatsSystem.GetSystem();
     }

     public void FinishGame()
     {
          HighScores.Add(CurrentGameScore);
          HighScores.Sort();
          HighScores.Reverse();
          if(HighScores.Count > 5)
               HighScores.RemoveRange(5, HighScores.Count - 5);
          CurrentGameScore = 0;
          SaveScores();
          _gameStatsSystem.FinishGame();
     }

     public void ResetHighScores()
     {
         HighScores.Clear();
         SaveScores();
     }

     private void SaveScores()
     {
         lock (this)
         {
             if (_saving) return;
             _saving = true;
             // Create something to save
             FinalizeSaveAsync(HighScores);
         }
     }

     private async void FinalizeSaveAsync(List<int> state)
     {
        await Task.Run(() =>
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.Create);
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(List<int>));
                        mySerializer.Serialize(fs, state);
                    }
                }
                catch (IsolatedStorageException)
                {
                    // Ideally show something to the user, but this is demo code :)
                }
            }

            _saving = false;
        });
     }

    /// <summary>
    /// Demonstrates how to deserialize an object from storage device
    /// </summary>
    private void LoadScores()
    {
        lock (this)
        {
            if (_loading) return;
            _loading = true;
            FinalizeLoadAsync();
        }
    }

    private async void FinalizeLoadAsync()
    {
        await Task.Run(() =>
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (storage.FileExists("HighScores.xml"))
                    {
                        using IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.Open);
                        {
                            XmlSerializer mySerializer = new XmlSerializer(typeof(List<int>));
                            HighScores = (List<int>)mySerializer.Deserialize(fs);
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                    // Ideally show something to the user, but this is demo code :)
                }
            }

            _loading = false;
        });
    }
}