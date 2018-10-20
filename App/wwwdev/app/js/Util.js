import moment from "moment";

let Util = {
    //https://stackoverflow.com/questions/400212/how-do-i-copy-to-the-clipboard-in-javascript
    copyTextToClipboard(text) {
        function fallbackCopyTextToClipboard(text) {
            var textArea = document.createElement("textarea");
            textArea.value = text;
            let appendTo = document.activeElement;
            appendTo.appendChild(textArea);
            textArea.focus();
            textArea.select();

            try {
                var successful = document.execCommand('copy');
                var msg = successful ? 'Copied!!' : 'Failed.';
                return Promise.resolve(msg);
            } catch (err) {
                return Promise.reject(err);
            }
            finally {
                appendTo.removeChild(textArea);
            }
        }
        function copyTextToClipboardInternal(text) {
            if (!navigator.clipboard) {
                return fallbackCopyTextToClipboard(text);
            }
            return navigator.clipboard.writeText(text).then(function () {
                return "Copied!";
            });
        }

        return copyTextToClipboardInternal(text);
    },
    getFirstNameLastInitial(fullName){
        let split = fullName.split(' ').filter(x => x.trim());
        let first = (split[0].length > 0 ? split[0] + ' ' : '');
        if(split.length <= 1){
            return first;
        }
        let last = split[split.length -1][0];
        if(last === undefined){
            return first;
        }
        return (first + last + '.')
    },
    minutesToTimeString(minutes, showSeconds){
        let seconds = Math.round(60*(minutes - Math.floor(minutes)));
        minutes = Math.floor(minutes);
        let showHours = Math.floor(minutes/60);
        let showMinutes = minutes % 60;
        if(showSeconds){
            return `${showHours}:${showMinutes <= 9? '0' : ''}${showMinutes}:${seconds <= 9? '0' : ''}${seconds}`;
        }
        else{
            return `${showHours}H ${showMinutes <= 9? '0' : ''}${showMinutes}M`;
        }
    },
    timeRangeToShortString(start, end){
        start = new moment(start);
        end = new moment(end);
        let now = new moment();
        let isToday = start.isSame(now, "day");

        if(start.isSame(now, "day")){
            return `${start.format('hA')} to ${end.format('hA')}`;
        }
        else if(start.isSame(end, "day")){
            return `${start.format('MM/DD hA')} to ${end.format('hA')}`;
        }
        else{
            return `${start.format('MM/DD hA')} to ${end.format('MM/DD hA')}`;
        }
    },
    parseInt(numberString, defaultValue){
        if(defaultValue === undefined){
            defaultValue = 0;
        }
        let ret = window.parseInt(numberString, 10);
        if(isNaN(ret)){
            return defaultValue;
        }
        return ret;
    },
    parseFloat(numberString, defaultValue){
        if(defaultValue === undefined){
            defaultValue = 0;
        }
        let ret = window.parseFloat(numberString);
        if(isNaN(ret)){
            return defaultValue;
        }
        return ret;
    },
    scrollTo(elementId){
        let timeout = 1;
        let doScroll = () => {
            let el = document.getElementById(elementId);
            if(el){
                el.scrollIntoView();
            }
            else{
                timeout *=2;
                if(timeout < 500){
                    setTimeout(doScroll, timeout);
                }
            }
        }
        doScroll();
    },
    significant(number){
        if(number > 10){
            return Math.round(number);
        }
        else{
            return number.toFixed(1);
        }
    },
};

export default Util;