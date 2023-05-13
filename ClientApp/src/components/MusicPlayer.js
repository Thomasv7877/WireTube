import React, { useEffect, useState } from 'react';

const MusicPlayer = () => {
  const [musicFiles, setMusicFiles] = useState([]);
  const [currentSongIndex, setCurrentSongIndex] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [progress, setProgress] = useState(0);

  const fetchMusicFiles = async () => {
    try {
      const response = await fetch('/ytApi');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      setMusicFiles(data);
      setCurrentSongIndex(0);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  const handlePlay = () => {
    setIsPlaying(true);
  };

  const handlePause = () => {
    setIsPlaying(false);
  };

  const handlePrevious = () => {
    setCurrentSongIndex(prevIndex => (prevIndex > 0 ? prevIndex - 1 : musicFiles.length - 1));
    setIsPlaying(true);
  };

  const handleNext = () => {
    setCurrentSongIndex(prevIndex => (prevIndex < musicFiles.length - 1 ? prevIndex + 1 : 0));
    setIsPlaying(true);
  };

  useEffect(() => {
    const updateProgress = () => {
      // Assuming the API provides the current progress of the song
      const currentProgress = // Get current progress from the API
      setProgress(currentProgress);
    };

    // Update progress every second
    const progressTimer = setInterval(updateProgress, 1000);

    return () => {
      clearInterval(progressTimer);
    };
  }, []);

  useEffect(() => {
    if (currentSongIndex !== null) {
      // Tell the API to play or pause the current song based on the isPlaying state
      // You can make a request to the API endpoint to control playback

      // Example request:
      // fetch(`https://api.example.com/music/play?index=${currentSongIndex}&isPlaying=${isPlaying}`)
      //   .then(response => {
      //     if (!response.ok) {
      //       throw new Error('Request failed');
      //     }
      //   })
      //   .catch(error => {
      //     console.error('Error:', error);
      //   });
    }
  }, [currentSongIndex, isPlaying]);

  return (
    <div>
      {musicFiles.length > 0 ? (
        <>
          <ul>
            {musicFiles.map((file, index) => (
              <li key={index}>{file}</li>
            ))}
          </ul>

          <p>Current Progress: {progress}%</p>

          <div>
            <button onClick={handlePrevious}>Previous</button>

            {isPlaying ? (
              <button onClick={handlePause}>Pause</button>
            ) : (
              <button onClick={handlePlay}>Play</button>
            )}

            <button onClick={handleNext}>Next</button>
          </div>
        </>
      ) : (
        <p>Loading music files...</p>
      )}
    </div>
  );
};

export default MusicPlayer;
