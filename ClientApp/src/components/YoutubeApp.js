import React, { useState } from "react";
//import { Download, Envelope } from "bootstrap-icons/icons";

export function YoutubeApp(){
    const [searchTerm, setSearchTerm] = useState('');
    //const apiKey = 'AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU';
    const apiKey = process.env.react_app_yt_api_key;
    const apiUrl = `https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=20&q=${searchTerm}&type=video&key=${apiKey}`;
    const [searchResults, setSearchResults] = useState([]);
    const testVar = "https://www.youtube.com/watch?v=1cBZcpSeiFc&pp=ygUTcGF5YmFjayBqYW1lcyBicm93bg%3D%3D";

    const handleSearch = async () => {
        try {
          const response = await fetch(apiUrl);
          const data = await response.json();
          console.log(data);
          setSearchResults(data.items);
        } catch (error) {
          console.error('Error:', error);
        }
      };

    const handlePostTest = async (vidUrl) => {
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

    return(
        <>
        <p>Find something on youtube</p>
        <input type="text" value={searchTerm} onChange={e => setSearchTerm(e.target.value)}/>
        <button type="button" className="btn btn-primary" onClick={handleSearch}>Search</button>
        <button type="button" className="btn btn-primary" onClick={() => handlePostTest(testVar)}>Post test & Clear results</button>
        <ul>
        {searchResults.map(result => 
        {
            var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
        return (
          <li key={result.id.videoId}>
            <img src={result.snippet.thumbnails.default.url}></img>
            <p>{result.snippet.title} <a href={vidLink}>Watch!</a> or <button type="button" className="btn" onClick={() => handlePostTest(vidLink)}><i className="icon bi-download"></i></button></p>
            </li>
        )})}
      </ul>
        </>
    );
}
/*
https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=20&q=lil xan colorblind&type=video&key=AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU
https://youtube.googleapis.com/youtube/v3/videos?part=snippet,statistics&id=wylMwU1wEOM&key=AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU

var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
var vidTitle = result.snippet.title
var channelTitle = result.channelTitle

*/