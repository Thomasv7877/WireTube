import React, { useState, useEffect, useRef } from 'react';
import '../custom.css'
import AudioVizualizer from './AudioVisualizer';

const MusicPlayerInReact = () => {
  const [musicFiles, setMusicFiles] = useState([]);
  const [currentSongIndex, setCurrentSongIndex] = useState(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const audioRef = useRef(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredSongs, setFilteredSongs] = useState([]);
  const [isShuffled, setIsShuffled] = useState(false);
  const previousSongIndexRef = useRef(null);
  var currentTime = useRef(0);
  const [audioState, setAudioState] = useState(audioRef.current);

  // fetch track info from backend
  const fetchMusicAll = async () => {
    try {
      const response = await fetch('/ytApi/tracks');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      setMusicFiles(data);
      setFilteredSongs(data);
      setCurrentSongIndex(0);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  // handle button events and playback behavior
  const handlePlay = () => {
    setIsPlaying(true);
  };

  const handlePause = () => {
    setIsPlaying(false);
  };

  const handlePrevious = () => {
    setCurrentSongIndex(prevIndex => (prevIndex > 0 ? prevIndex - 1 : filteredSongs.length - 1));
    setIsPlaying(true);
  };

  const handleNext = () => {
    if(isShuffled){
      setCurrentSongIndex(Math.floor(Math.random() * filteredSongs.length));
    }else{
      setCurrentSongIndex(prevIndex => (prevIndex < filteredSongs.length - 1 ? prevIndex + 1 : 0));
    }
    setIsPlaying(true);
  };
  const handleShuffle = () => {
    setIsShuffled(!isShuffled);
  };

  const handleDoubleClick = (index) => {
    setCurrentSongIndex(index);
    setIsPlaying(true);
  }
  // handle search bar behavior
  const handleSearch = (event) => {
    const query = event.target.value;
    setSearchQuery(query);
    setIsPlaying(false);
  
    const filtered = musicFiles.filter((song) =>
      (song.fileName + song.title + song.artist + song.album).toLowerCase().includes(query.toLowerCase())
    );
    setFilteredSongs(filtered);
  };

  // Fetch music files when the component mounts
  useEffect(() => {
    fetchMusicAll();
  }, []);

  // Handle playback when the current song index changes
  useEffect(() => {
    if (currentSongIndex !== null) {
      if (isPlaying) {
        // Play the audio, if same song index resume position (pause behavior)
        if(previousSongIndexRef.current === currentSongIndex){
          audioRef.current.currentTime = currentTime.current;
        }
        audioRef.current.play();
        setAudioState(audioRef.current); // use state for active audio to update visualizer
      } else {
        // Pause the audio, save song position
        currentTime.current = audioRef.current.currentTime;
        audioRef.current.pause();
      }
      previousSongIndexRef.current = currentSongIndex;
    }
  }, [currentSongIndex, isPlaying, filteredSongs]);

  // helper function for settings src on Audio element
  const getAudioSource = () => {
    var song = filteredSongs[currentSongIndex].fileName;
    return isPlaying? `ytApi/audio?fileName=${song}` : null;
  }

  // music library, search bar, list view and media controls fixed at the bottom. Visualizer is seperate component, shown above the audio bar
  return (
    <>
    <p id="lib-title">Music Library</p>
    <div className='al-container'>
      {musicFiles.length > 0 ? (
        <>
          <input id='search-lib' type="text" value={searchQuery} onChange={handleSearch} placeholder="Search songs..."/>
          <div className='scroll-pane'>
            <div className='content-wrapper'>
            <table className="table table-striped" aria-labelledby="tableLabel">
              <thead>
                <tr>
                  <th>Nr</th><th>FileName</th><th>Title</th><th>Artist</th><th>Album</th><th>Duration</th>
                </tr>
              </thead>
              <tbody>
              {filteredSongs.map((song, index) => (
              <tr key={index} onDoubleClick={() => handleDoubleClick(index)} className={currentSongIndex === index && isPlaying ? 'active' : ''}>
                <td>{index}</td>
                <td>{song.fileName}</td>
                <td>{song.title}</td>
                <td>{song.artist}</td>
                <td>{song.album}</td>
                <td>{song.duration}</td>
              </tr>
            ))}
              </tbody>
            </table>
          </div>
          </div>
          
          <div id='media-control'>
          {/*<canvas className='visCanvas' ref={canvasRef}></canvas>*/}
          <AudioVizualizer audioState={audioState}></AudioVizualizer>
          <audio id="audio-player" ref={audioRef} src={getAudioSource()} onEnded={handleNext} controls />
            <button onClick={handlePrevious}>Previous</button>

            {isPlaying ? (
              <button onClick={handlePause}>Pause</button>
            ) : (
              <button onClick={handlePlay}>Play</button>
            )}

            <button onClick={handleNext}>Next</button>
            <button onClick={handleShuffle} style={{backgroundColor: isShuffled && '#00b6f0'}}>Shuffle</button>
          </div>
        </>
      ) : (
        <p>Loading music files...</p>
      )}
    </div>
    </>
  );
};

export default MusicPlayerInReact;
