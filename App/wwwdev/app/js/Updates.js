import SignalR from 'signalr';
import Env from './Environment';
import $script from 'scriptjs';
import axios from 'axios';

let signalrStarted = false;
function doHubActionAfterLoad(doThis){
    let tries = 0;
    let callThing = () => {
        if(signalrStarted){
            doThis();
        }
        else{
            tries++;
            if(tries > 10){
                throw new Error("Signalr hub never loaded.");
            }
            else{
                setTimeout(callThing, 500);
            }
        }
    }
    callThing();
}

var subscribers = new Set();
var subscribedGroups = [];
var Updates = {

    /*  register:
        NOTE: 'this' will be set to the subscriber object when any of its functions are called.
        The subscriber object is typically the vm for the view.

        subscriber : {
            mounted(){
                Updates.register(this, ["group1", "group2"]);
            },
            destroyed(){
                Updates.remove(this);
            },
            processUpdate : function(group, cmd){
            },
        }
    */
    register(subscriber, groups) {
        doHubActionAfterLoad(()=>{
            subscriber.__subGroups = groups;
            groups.forEach((group) => {
                let numSubscribers = subscribedGroups[group];
                if(numSubscribers === undefined){
                    numSubscribers = 0;
                }

                if(numSubscribers === 0){
                    $.connection.updateHub.server.subscribe(group);
                }
                subscribedGroups[group] = (numSubscribers + 1);
            });
            subscribers.add(subscriber);
        })
    },
    remove(subscriber) {
        doHubActionAfterLoad(() => {
            let groups = subscriber.__subGroups;
            if(groups === undefined){
                //Would happen if signalr never loaded. Overall means there's nothing to unsubscribe.
                return;
            }

            if(subscribers.delete(subscriber)){
                groups.forEach((group) => {
                    let numSubscribers = subscribedGroups[group];
                    if(numSubscribers === undefined || numSubscribers <= 0){
                        numSubscribers = 0;
                    }
                    else{
                        numSubscribers--;
                    }
                    if(numSubscribers === 0){
                        $.connection.updateHub.server.unsubscribe(group);
                    }
                    subscribedGroups[group] = numSubscribers;
                });
            }
        })
    },
};

var lastUpdateReceived = new Date().getTime();
let haveReceivedUpdate = false;
let inFailedState = false;
var checkIfNeedRestart = function () {
    if(!haveReceivedUpdate){
        //If updates were never working, let's not raise an alarm.
        //Maybe we should?
        return;
    }
    var now = new Date().getTime();
    if ((now - lastUpdateReceived) > (1000 * 30)) {
        if(!inFailedState){
            //Send a single warning when we enter the failed state.
            alert("Server connection lost. Please refresh the application to restore real-time updates.");
        }
        inFailedState = true;
    }
    else{
        inFailedState = false;
    }
}

setInterval(checkIfNeedRestart, 5000);

function loadSignalr() {
    $script(Env.signalrRoot + '/hubs', function () {
        $.connection.hub.url = Env.signalrRoot;
        var updateHub = $.connection.updateHub;
        updateHub.client.receiveUpdates = function (updates) {
            lastUpdateReceived = new Date().getTime();
            haveReceivedUpdate = true;
            updates.forEach(function (update) {
                try {
                    let splitUpdate = update.split('|');
                    let group = splitUpdate[0];
                    let cmd = splitUpdate[1];
                    subscribers.forEach(function (subscriber) {
                        try {
                            if(subscriber.__subGroups && subscriber.__subGroups.includes(group)){
                                subscriber.processUpdate(group, cmd);
                            }
                        }
                        catch (ex) {
                            console.log("Exception Ignored: " + ex);
                        }
                    });
                }
                catch (ex) {
                    console.log("Exception Ignored: " + ex);
                }
            });
        };

        $.connection.hub.start().done(function () {
            signalrStarted = true;
            console.log("SignalR Started Successfully");
        });

    });
}

loadSignalr();
export default Updates;