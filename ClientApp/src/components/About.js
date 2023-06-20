import React from "react";

export function About(){

    return(<div className="col-md-6 offset-md-1" style={{"marginTop": "5%"}}>
    <h3>About</h3>
    <p>Search youtube, rip audio (using yt-dlp) and playback in a music library view.</p>
    <p id="signature">WireTube made by <span id="firstname">Thomas VdH</span></p>
    </div>)
}