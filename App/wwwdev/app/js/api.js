import axios from 'axios';
import AuthState from './AuthState.js';
import store from './store.js';

function getOrPost(url, payload, getPostEtc) {
    return getPostEtc(url, payload).then(function (response) {
        if (response.data === null || response.data === undefined) {
            throw new Error("Implementation Error. Null returned from: " + url);
        }
        if (response.data.tokenNotFound) {
            AuthState.logOut(store);
            throw new Error("Auth token is not valid. Must log back in. url: " + url);
        }
        if (response.data.notAuthorized) {
            throw new Error("User not authorized for: " + url);
        }
        if (response.data.errorOccurred) {
            throw new Error(response.data.errorMsg + " | url: " + url);
        }
        return response.data.data;
    });
}

const api = {
    get: function (url, payload) {
        return getOrPost(url, payload, axios.get);
    },
    post: function (url, payload) {
        return getOrPost(url, payload, axios.post);
    },
    put: function (url, payload){
        return getOrPost(url, payload, axios.put);
    },
    delete: function(url, payload){
        return getOrPost(url, payload, axios.delete);
    }
};

export default api;