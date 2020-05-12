import * as signalR from '@microsoft/signalr';
import Env from './Environment';

let connection = null;
let subscribers = new Set();
let subscribedGroups = [];
let signalrReady = false;
let currentServerId = null;

let lastUpdateReceived = new Date().getTime();
let haveReceivedUpdate = false;
let inFailedState = false;

loadSignalr();
function loadSignalr() {
    connection = new signalR.HubConnectionBuilder().withUrl(`${Env.signalrRoot}`).build();
    connection.on('receiveUpdates', updates => {
        lastUpdateReceived = new Date().getTime();
        haveReceivedUpdate = true;
        updates.forEach(function (update) {
            try {
                let splitUpdate = update.split('|');
                let group = splitUpdate[0];
                let cmd = splitUpdate[1];
                if(group === 'ping'){
                    if(currentServerId === null){
                        currentServerId = cmd;
                    }
                    else if(currentServerId !== cmd){
                        //We've connected to a different webserver:
                        currentServerId = cmd;
                        renewSubscriptionsOnNewWebServer();
                    }
                    return;
                }
                subscribers.forEach(function (subscriber) {
                    try {
                        if(subscriber.__subscribedGroups && subscriber.__subscribedGroups.includes(group)){
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
    });

    connection.start().then(() => {
        signalrReady = true;
        console.log("SignalR Started Successfully");
    });

    function renewSubscriptionsOnNewWebServer(){
        let subscriberList = new Set();
        subscribers.forEach(function (subscriber) {
            try {
                if(subscriber.__subscribedGroups){
                    subscriber.__subscribedGroups.forEach(group => {
                        if(group && (typeof(group) === 'string')){
                            subscriberList.add(group);
                        }
                    });
                }
            }
            catch (ex) {
                console.log("exception ignored during signalr renew: " + ex);
            }
        });
        connection.invoke('renew', Array.from(subscriberList).join('|'));
    }
}

setInterval(checkIfNeedRestart, 5000);
function checkIfNeedRestart() {
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

let Updates = {
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
        Updates.whenSignalrReady(()=>{
            subscriber.__subscribedGroups = groups;
            groups.forEach((group) => {
                let numSubscribers = subscribedGroups[group];
                if(numSubscribers === undefined){
                    numSubscribers = 0;
                }

                if(numSubscribers === 0){
                    connection.invoke("subscribe", group);
                }
                subscribedGroups[group] = (numSubscribers + 1);
            });
            subscribers.add(subscriber);
        })
    },
    remove(subscriber) {
        Updates.whenSignalrReady(() => {
            let groups = subscriber.__subscribedGroups;
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
                        connection.invoke('unsubscribe', group);
                    }
                    subscribedGroups[group] = numSubscribers;
                });
            }
        })
    },
    whenSignalrReady(doThis){
        let tries = 0;
        doThisIfReady();
        function doThisIfReady(){
            if(signalrReady){
                doThis();
            }
            else{
                tries++;
                if(tries > 10){
                    throw new Error("Signalr hub never loaded.");
                }
                else{
                    setTimeout(doThisIfReady, 500);
                }
            }
        }
    }
};

export default Updates;