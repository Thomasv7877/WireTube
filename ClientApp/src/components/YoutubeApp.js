import React, { useState } from "react";

export function YoutubeApp(){
    const [searchTerm, setSearchTerm] = useState('');
    //const apiKey = 'AIzaSyDnMozDltngJNf45Dnel-Xxo1Gm-Q0_uUU';
    const apiKey = process.env.react_app_yt_api_key;
    const apiUrl = `https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=20&q=${searchTerm}&type=video&key=${apiKey}`;
    const [searchResults, setSearchResults] = useState([]);
    const testVar = "Beginning to End";

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

    const handlePostTest = async () => {
      setSearchResults([]);
      try {
        const response = await fetch("/ytApi", {
          method: 'POST',
          headers: {'Content-Type': 'application/json'},
          body: JSON.stringify({message: testVar})
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
        <button type="button" className="btn btn-primary" onClick={handlePostTest}>Post test & Clear results</button>
        <ul>
        {searchResults.map(result => 
        {
            var vidLink = `https://www.youtube.com/watch?v=${result.id.videoId}`;
        return (
          <li key={result.id.videoId}><p>{result.snippet.title} <a href={vidLink}>Watch!</a></p></li>
        )})}
      </ul>
        </>
    );
}