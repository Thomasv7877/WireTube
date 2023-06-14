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
  //const canvasRef = useRef(null);
  const previousSongIndexRef = useRef(null);
  var currentTime = useRef(0);

  //const [analyzerData, setAnalyzerData] = useState(null);
  //const audioCtxRef = useRef(null);
  //const analyzerRef = useRef(null);
  //const sourceRef = useRef(null);
  const [audioState, setAudioState] = useState(audioRef.current);

  /*const fetchMusicList = async () => {
    try {
      const response = await fetch('/ytApi/tracknames');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      setMusicFiles(data);
      setFilteredSongs(data);
      //handleSearch(data)
      setCurrentSongIndex(0);
    } catch (error) {
      console.error('Error:', error);
    }
  };*/
  const fetchMusicAll = async () => {
    try {
      const response = await fetch('/ytApi/tracks');
      if (!response.ok) {
        throw new Error('Request failed');
      }
      const data = await response.json();
      //console.log(data[0]);
      setMusicFiles(data);
      setFilteredSongs(data);
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
  const handleSearch = (event) => {
    const query = event.target.value;
    setSearchQuery(query);
    setIsPlaying(false);
  
    /*const filtered = musicFiles.filter((song) =>
      song.toLowerCase().includes(query.toLowerCase())
    );*/
    console.log(musicFiles[0]);
    const filtered = musicFiles.filter((song) =>
      (song.fileName + song.title + song.artist + song.album).toLowerCase().includes(query.toLowerCase())
    );
    setFilteredSongs(filtered);
  };

  // Fetch music files when the component mounts
  useEffect(() => {
    //fetchMusicList();
    fetchMusicAll();
  }, []);
/*
  const audioAnalyzer = () => {
    // create a new AudioContext
    audioCtxRef.current = new (window.AudioContext || window.webkitAudioContext)();
    const audioCtx = audioCtxRef.current;
    // create an analyzer node with a buffer size of 2048
    analyzerRef.current = audioCtx.createAnalyser();
    const analyzer = analyzerRef.current;
    analyzer.fftSize = 256; // original was 2048

    const bufferLength = analyzer.frequencyBinCount;
    const dataArray = new Uint8Array(bufferLength);
    sourceRef.current = audioCtx.createMediaElementSource(audioRef.current);
    var source = sourceRef.current;
    console.log("Created a source");
    source.connect(analyzer);
    source.connect(audioCtx.destination);
    source.onended = () => {
        source.disconnect();
        console.log("Disconnected a source");
    };

    // set the analyzerData state with the analyzer, bufferLength, and dataArray
    setAnalyzerData({ analyzer, bufferLength, dataArray });
  };

  function animateBars(analyser, canvas, canvasCtx, dataArray, bufferLength) {
    // Analyze the audio data using the Web Audio API's `getByteFrequencyData` method.
    analyser.getByteFrequencyData(dataArray);
  
    // Set the canvas fill style to black.
    canvasCtx.fillStyle = '#000';
  
    // Calculate the height of the canvas.
    //const HEIGHT = canvas.height / 2;
    const HEIGHT = canvas.height;
  
    // Calculate the width of each bar in the waveform based on the canvas width and the buffer length.
    var barWidth = Math.ceil(canvas.width / bufferLength); // original was * 2.5
  
    // Initialize variables for the bar height and x-position.
    let barHeight;
    let x = 0;
  
    // Loop through each element in the `dataArray`.
    for (var i = 0; i < bufferLength; i++) {
      // Calculate the height of the current bar based on the audio data and the canvas height.
      barHeight = (dataArray[i] / 255) * HEIGHT;
  
      // Generate random RGB values for each bar.
      //const maximum = 10;
      //const minimum = -10;
      //var r = 242 + Math.floor(Math.random() * (maximum - minimum + 1)) + minimum;
      //var g = 104 + Math.floor(Math.random() * (maximum - minimum + 1)) + minimum;
      //var b = 65 + Math.floor(Math.random() * (maximum - minimum + 1)) + minimum;
      var r = 0; var g = 169; var b = 223;
  
      // Set the canvas fill style to the random RGB values.
      canvasCtx.fillStyle = 'rgb(' + r + ',' + g + ',' + b + ')';
  
      // Draw the bar on the canvas at the current x-position and with the calculated height and width.
      //canvasCtx.fillRect(x, HEIGHT - barHeight, barWidth, barHeight);
      canvasCtx.fillRect(x, canvas.height - barHeight, barWidth, barHeight);
  
      // Update the x-position for the next bar.
      x += barWidth + 1;
    }
  }

  // Function to draw the waveform
  const drawCallback = useCallback(() => {
      const canvas = canvasRef.current;
      if (!canvas || !analyzerData.analyzer) return;
      const canvasCtx = canvas.getContext("2d");
  
      const animate = () => {
        requestAnimationFrame(animate);
        //canvas.width = canvas.width;
        canvasCtx.clearRect(0, 0, canvas.width, canvas.height);
        animateBars(analyzerData.analyzer, canvas, canvasCtx, analyzerData.dataArray, analyzerData.bufferLength);
      };
  
      animate();
  }, [analyzerData]);
  

  // Effect to draw the waveform on mount and update
  useEffect(() => {
    drawCallback();
  }, [analyzerData, drawCallback]);
*/
  // Handle playback when the current song index changes
  useEffect(() => {
    if (currentSongIndex !== null) {
      if (isPlaying) {
        // Play the audio
        // Assumes musicFiles contain the full URLs to the audio files
        //const audio = new Audio(`ytApi/audio?fileName=${musicFiles[currentSongIndex]}`);
        //const audio = document.getElementById('audio-player');
        //audio.play();
        if(previousSongIndexRef.current === currentSongIndex){
          audioRef.current.currentTime = currentTime.current;
        }
        //audioRef.current.volume = 1; // workaround volume verhoogt elke song
        audioRef.current.play();
        /*if(sourceRef.current){
          sourceRef.current.disconnect(); // indien vorige ref niet disconnect zal volume trapgewijs verhogen..
        }
        audioAnalyzer();*/
      } else {
        // Pause the audio
        //const audio = document.getElementById('audio-player');
        //audio.pause();
        currentTime.current = audioRef.current.currentTime;
        audioRef.current.pause();
      }
      previousSongIndexRef.current = currentSongIndex;
    }
  }, [currentSongIndex, isPlaying, filteredSongs]);

  const getAudioSource = () => {
    var song = filteredSongs[currentSongIndex].fileName;
    setAudioState(audioRef.current);
    return isPlaying? `ytApi/audio?fileName=${song}` : null;
  }

  return (
    <div className='al-container'>
      {musicFiles.length > 0 ? (
        <>
          <input id='search-lib' type="text" value={searchQuery} onChange={handleSearch} placeholder="Search songs..."/>
          <div className='scroll-pane'>
            <div className='content-wrapper'>
          {/*<ol id='audio-library'>
            {filteredSongs.map((file, index) => (
              <li key={index} onDoubleClick={() => handleDoubleClick(index)} className={currentSongIndex === index && isPlaying ? 'active' : ''}>{file}</li>
            ))}
            </ol>
            */}
            <table className="table table-striped" aria-labelledby="tableLabel">
              <thead>
                <tr>
                  <th>Nr</th>
                  <th>FileName</th>
                  <th>Title</th>
                  <th>Artist</th>
                  <th>Album</th>
                  <th>Duration</th>
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
          <audio id="audio-player" ref={audioRef} src={() => getAudioSource()} onEnded={handleNext} controls />
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
  );
};

export default MusicPlayerInReact;
