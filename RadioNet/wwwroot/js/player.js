function getAudioElement() {
    return document.getElementById("audio");
}

var hls = new Hls();
export function play(stationUrl, codec) {
    const audio = getAudioElement();

    if (stationUrl.includes('.m3u8') && Hls.isSupported()) {

        // MEDIA_ATTACHED event is fired by hls object once MediaSource is ready
        /*hls.on(Hls.Events.MEDIA_ATTACHED, function () {
            console.log('audio and hls.js are now bound together !');
        });
        hls.on(Hls.Events.MANIFEST_PARSED, function (event, data) {
            console.log(
                'manifest loaded, found ' + data.levels.length + ' quality level'
            );
        });*/
        
        audio.src = audio.type = null;
        hls.loadSource(stationUrl);
        hls.attachMedia(audio);
    } else {
        hls.detachMedia(audio);
        audio.src = stationUrl;
        audio.type = `audio/${codec}`;
    }

    audio.load();
    audio.play();
}

export function stop() {
    const audio = getAudioElement();

    audio.stop();
}
