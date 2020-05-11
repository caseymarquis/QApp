import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

const store = new Vuex.Store({
    state: {
        loggedIn: false,
        loginMessage: "",
        },
        pageFluid: false,
        pageTitle: "",
        screenHeight: 1000,
        search: {
            show: false,
            placeholder: true,
            text: "",
        },
        slide: {
            visible: false,
            hasEntered: false,
        },
        user: {
            id: "",
            name: "",
            userName: "",
            isAdmin: false,
        },
    },
    mutations: {
        login(state, user) {
            state.loggedIn = true;
            state.loginMessage = "";
            state.user.name = user.name;
            state.user.id = user.id;
            state.user.isAdmin = user.isAdmin;
            state.user.userName = user.userName;
        },
        loginFailed(state, message) {
            state.loginMessage = message;
        },
        logout(state) {
            state.loginMessage = "";
            state.loggedIn = false;
        },
        clearLoginMessage(state) {
            state.loginMessage = "";
        },
        setPageTitle(state, title) {
            if (title === null || title === undefined) {
                title = "";
            }
            state.pageTitle = title;
        },
        setPageFluid(state, value){
            state.pageFluid = value;
        },
        setScreenHeight(state, height){
            state.screenHeight = height;
        },
        setupSearch(state, search){
            if(!search.placeholder){
                search.placeholder = "Search..."
            }
            if(!search.text){
                search.text = "";
            }
            if(search.show === undefined || search.show === null){
                search.show = true;
            }
            state.search = search;
        },
        setSearchText(state, text){
            if(!text){
                text = "";
            }
            state.search.text = text;
        },
        setupSlide(state){
            state.slide.visible = false;
        },
        toggleSlide(state){
            state.slide.visible = !state.slide.visible;
            state.slide.hasEntered = true;
        },
    }
});

export default store;