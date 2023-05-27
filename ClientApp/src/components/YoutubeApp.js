import React, { useState, useEffect } from "react";
//import { Download, Envelope } from "bootstrap-icons/icons";
import '../custom.css'

export function YoutubeApp(){
    const [searchTerm, setSearchTerm] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    //const apiKey = 'AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU';
    const apiKey = process.env.react_app_yt_api_key;
    const apiUrl = `https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=10&q=${searchTerm}&type=video&key=${apiKey}`;
    const [searchResults, setSearchResults] = useState([]);
    //const testVar = "https://www.youtube.com/watch?v=1cBZcpSeiFc&pp=ygUTcGF5YmFjayBqYW1lcyBicm93bg%3D%3D";

    const handleSearch = async () => {
        try {
          setIsLoading(true);
          const response = await fetch(apiUrl);
          const data = await response.json();
          for(var i = 0; i < data.items.length; i++){
            var detailUrl = `https://youtube.googleapis.com/youtube/v3/videos?part=snippet,statistics&id=${data.items[i].id.videoId}&key=${apiKey}`;
            const subResponse = await fetch(detailUrl);
            const subData = await subResponse.json();
            //console.log(subData.items[0].statistics.viewCount);
            data.items[i]['views'] = subData.items[0].statistics.viewCount;
          }
          //console.log(data.items[0]);
          setSearchResults(data.items);
          setIsLoading(false);
        } catch (error) {
          console.error('Error:', error);
        }
      };

    const handleDownload = async (vidUrl) => {
      setSearchResults([]);
      try {
        const response = await fetch("/ytApi", {
          method: 'POST',
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify(vidUrl)
            });
        const data = await response.json();
        console.log(data);
        console.log("test");
      } catch (error) {
        //console.error('Error:', error);
      }
    }
    const handleKeyDown = (event) => {
      if(event.key === 'Enter'){
        handleSearch();
      }
    }

    /*const handleEvent = (event) => {
      console.log('Received event:', event.data);
      // Process the event data as needed
    };

    useEffect(() => {
      const eventSource = new EventSource('/ytApi/dlprogresssocket');
    
      eventSource.addEventListener('update', handleEvent);
      eventSource.addEventListener('error', (error) => {
        console.error('SSE error:', error);
      });
    
      return () => {
        eventSource.close(); // Clean up the EventSource instance when the component is unmounted
      };
    }, []);*/

    /*useEffect(() => {
      // Establish WebSocket connection to SSE endpoint
      const socket = new WebSocket('/ytApi/dlprogresssocket');
  
      // Handle incoming progress updates
      socket.onmessage = (event) => {
        //const progress = JSON.parse(event.data);
        // Update your React component state or UI based on progress
        console.log("got something..");
      };
  
      // Clean up the WebSocket connection
      return () => {
        socket.close();
      };
    }, []);*/

    useEffect(() => {
      const eventSource = new EventSource('/ytApi/dlprogresssocket');
      eventSource.addEventListener('update', (event) => {
        //const eventData = JSON.parse(event.data);
        // Process the received event data (e.g., update UI)
        console.log("got something..");
      });
      eventSource.addEventListener('error', (event) => {
        const eventData = JSON.parse(event.data);
        // Process the received event data (e.g., update UI)
        console.log("something went wrong..");
      });
    }, []);

    return(
        <>
        <p>Find something on youtube</p>
        <input id="searchYt" type="text" value={searchTerm} onChange={e => setSearchTerm(e.target.value)} onKeyDown={handleKeyDown} placeholder="Search songs..."/>
        {/*<button type="button" className="btn btn-primary btn-sm" onClick={handleSearch}>Search</button>
        <button type="button" className="btn btn-primary" onClick={() => handlePostTest(testVar)}>Post test & Clear results</button>*/}
        {!isLoading ? (
        <ul id="ytResults">
        {searchResults.map(result => 
        {
            var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
        return (
          <li key={result.id.videoId} className="clearfix">
            <a href={vidLink}><img src={result.snippet.thumbnails.default.url} alt={result.snippet.title}></img></a>
            <h5>{result.snippet.title}</h5>
            <p>{result.snippet.channelTitle} - {formatViews(result.views)} views</p>
            <button type="button" className="btn" onClick={() => handleDownload(vidLink)}><i className="icon bi-download"></i></button>
            </li>
        )})}
      </ul>) : (<div className="spinner-grow text-primary m-5" role="status">
  <span className="sr-only">test</span>
</div>)}
        </>
    );
}

function formatViews(views){
  if(views > 1000000){
    return parseInt(Math.round(views / 1000000)) + " mln.";
  } else if (views > 1000){
    return parseInt(Math.round(views / 1000)) + "k";
  } else {
    return views;
  }
}
/*
https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=20&q=lil xan colorblind&type=video&key=AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU
https://youtube.googleapis.com/youtube/v3/videos?part=snippet,statistics&id=wylMwU1wEOM&key=AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU

var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
var vidTitle = result.snippet.title
var channelTitle = result.snippet.channelTitle
var imgSrc = result.snippet.thumbnails.default.url
var vidId = result.id.videoId
*/