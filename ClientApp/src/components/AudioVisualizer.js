import React, { useState, useEffect, useRef, useCallback } from 'react';
import '../custom.css';

export default function AudioVizualizer({audioState}){
    const canvasRef = useRef(null);
    const [analyzerData, setAnalyzerData] = useState(null);
    const audioCtxRef = useRef(null);
    const analyzerRef = useRef(null);
    const sourceRef = useRef(null);

    /*
    visualizer: audio context with analyzer on the audio source, each time new audio data is available the canvas gets updated
    */
    
      // draw bars in the canvas based on analyzerData
      function animateBars(analyser, canvas, canvasCtx, dataArray, bufferLength) {
        // Analyze the audio data using the Web Audio API's `getByteFrequencyData` method.
        analyser.getByteFrequencyData(dataArray);
      
        // Set the canvas fill style to black.
        canvasCtx.fillStyle = '#000';
      
        // Calculate the width of each bar in the waveform based on the canvas width and the buffer length.
        var barWidth = Math.ceil(canvas.width / bufferLength); // original was * 2.5, edit depending on fftsize
      
        // Initialize variables for the bar height and x-position.
        let barHeight;
        let x = 0;
      
        // Loop through each element in the `dataArray`.
        for (var i = 0; i < bufferLength; i++) {
          // Calculate the height of the current bar based on the audio data and the canvas height.
          barHeight = (dataArray[i] / 255) * canvas.height;
      
          // blue bar color, same as audio playback element
          var r = 0; var g = 169; var b = 223;
          canvasCtx.fillStyle = 'rgb(' + r + ',' + g + ',' + b + ')';
      
          // Draw the bar on the canvas at the current x-position and with the calculated height and width.
          canvasCtx.fillRect(x, canvas.height - barHeight, barWidth, barHeight);
      
          // Update the x-position for the next bar.
          x += barWidth + 1;
        }
      }
    
      // Function to draw the waveform each time the analyzerData changes, animation frame to smooth transition, canvas cleared each update
      const drawCallback = useCallback(() => {
          const canvas = canvasRef.current;
          if (!canvas || !analyzerData) {
            // do nothing if no canvas or data
          } else {
            const canvasCtx = canvas.getContext("2d");
        
            const animate = () => {
                requestAnimationFrame(animate);
                canvasCtx.clearRect(0, 0, canvas.width, canvas.height);
                animateBars(analyzerData.analyzer, canvas, canvasCtx, analyzerData.dataArray, analyzerData.bufferLength);
            };
        
            animate();
        }
      }, [analyzerData]);
      
    // each time track changes: use Audio Web API to create a new context and an analyzer on the contecxt (to generate wave data)
    const audioAnalyzerCallback = useCallback(() => {
        audioCtxRef.current = new (window.AudioContext || window.webkitAudioContext)();
        const audioCtx = audioCtxRef.current;
        // create an analyzer node with a buffer size of.. 256 or 2048 are common
        analyzerRef.current = audioCtx.createAnalyser();
        const analyzer = analyzerRef.current;
        analyzer.fftSize = 256; // original was 2048
    
        const bufferLength = analyzer.frequencyBinCount;
        const dataArray = new Uint8Array(bufferLength);
        sourceRef.current = audioCtx.createMediaElementSource(audioState);
        var source = sourceRef.current;
        source.connect(analyzer);
        source.connect(audioCtx.destination);
        source.onended = () => {
            source.disconnect();
            console.log("Disconnected a source");
        };
    
        // set the analyzerData state with the analyzer, bufferLength, and dataArray
        setAnalyzerData({ analyzer, bufferLength, dataArray });
    }, [audioState]);

      // Effect to draw the waveform on mount and update
      useEffect(() => {
        drawCallback();
      }, [analyzerData, drawCallback]);

      useEffect(() => {
        if(sourceRef.current){
            sourceRef.current.disconnect(); // unexcpected behavior if previous ref is not disconnected, such as increasing volume each time a new context is made..
          }
          if(audioState){
            audioAnalyzerCallback();
          }
      }, [audioState, audioAnalyzerCallback]);

      return(
        <canvas className='visCanvas' ref={canvasRef}></canvas>
      );

}