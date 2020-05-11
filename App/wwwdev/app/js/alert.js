import store from './store.js';

let activePromiseResolver = null;

let zAlert = {
    alert(title, text){
        return zAlert.generic(title, text, false);
    },
    confirm(title, text){
        return zAlert.generic(title, text, true);
    },
    generic(title, text, showCancel){
        store.commit('setModal', {
            show: true,
            title,
            text,
            showCancel: showCancel,
        })
        return new Promise((resolve, reject) => {
            activePromiseResolver = resolve;
        });
    },
    resolve(result){
        if(activePromiseResolver){
            activePromiseResolver(result);
            activePromiseResolver = null;
        }
        store.commit('setModal', {
            show: false,
            title: "",
            text: "",
            showCancel: false,
        })
    }
}

export default zAlert;