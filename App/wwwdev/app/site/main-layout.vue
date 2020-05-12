<template>
  <div id="main-layout">
    <app-modal v-if="$store.state.modal.show" :title="$store.state.modal.title" :show-cancel="$store.state.modal.showCancel" v-on:close="closeMainModal">
      <p v-text="$store.state.modal.text"></p>
    </app-modal>

    <slide-menu :style="slideDivStyle"></slide-menu>

    <div id="the-nav">
      <div class="nav-section">
        <router-link to="/" id="text-logo">
            <h1>
                QApp
            </h1>
          <!-- img :src="imgLogo" id="the-logo" / -->
        </router-link>
        <h2 id="page-title" class="visible-lg" v-text="pageTitle"></h2>
        <button v-if="loggedIn" class="btn btn-primary" v-on:click="toggleSlide">
          <span class="glyphicon glyphicon-list"></span>
        </button>
      </div>

      <search-bar id="nav-search" :wide="true" v-if="loggedIn && $store.state.search.show" v-model="searchText" placeholder="Search..."></search-bar>

      <div v-if="loggedIn" class="nav-section">
        <router-link v-text="userFirstName" :to="'/user/' + $store.state.user.id" class="nav-link"></router-link>
        <a href="javascript:void(0)" v-on:click="logout" class="nav-link">Log Out</a>
        <router-link to="/" class="nav-link">Home</router-link>
      </div>
    </div>

    <div id="site-container" :style="mainDivStyle">
      <page-home style="height: 100%;" v-if="!loggedIn"></page-home>
      <router-view v-else style="height: 100%;"></router-view>
    </div>
  </div>
</template>

<script>
import PageHome from "../page-home/page-home.vue";
import AppModal from "../shared-components/app-modal.vue";
import SearchBar from "../shared-components/search-bar.vue";
import SlideMenu from "./slide-menu.vue";
//import imgLogo from "../img/logo.png";

import AuthState from "../js/AuthState.js";
import SetupPage from "../js/SetupPage.js";
import router from "../js/router.js";

export default {
    data() {
        return {
            //imgLogo,
            wasMounted: false
        };
    },
    mounted() {
        this.wasMounted = true;
    },
    created() {
        this.updateScreenHeight();
        setTimeout(() => {
            this.updateScreenHeight();
        }, 100);
        setInterval(() => {
            this.updateScreenHeight();
        }, 500);
        AuthState.tryToLogInWithOldAuthToken(this.$store);
    },
    computed: {
        loggedIn() {
            return this.$store.state.loggedIn;
        },
        userFirstName() {
            let name = this.$store.state.user.name;
            if (name == undefined || name == null) {
                return "";
            }
            name = name.trim();
            let parts = name.split(" ");
            let ret = "";
            if (parts.length === 1) {
                return parts[0];
            }
            for (let i = 0; i < parts.length; i++) {
                let part = parts[i];
                if (i === parts.length - 1) {
                    ret += " " + part;
                } else if (part.length > 0) {
                    ret += part[0] + ".";
                }
            }
            return ret;
        },
        pageTitle() {
            return this.$store.state.pageTitle;
        },
        searchText: {
            get() {
                return this.$store.state.search.text;
            },
            set(text) {
                if (text === undefined || text === null) {
                    return;
                }
                router.appendQuery(this, { search: text });
                this.$store.commit("setSearchText", text);
            }
        },
        mainDivStyle(){
            return { height: `${this.$store.state.screenHeight}px` } 
        },
        slideDivStyle(){
            return { height: `${this.$store.state.screenHeight}px` } 
        }
    },
    methods: {
        logout() {
            if (this.wasMounted) {
                AuthState.logOut(this.$store);
            }
        },
        toggleSlide() {
            this.$store.commit("toggleSlide");
        },
        updateScreenHeight() {
            let navEl = document.getElementById('the-nav');
            if(!navEl){
                return;
            }
            let newVal = window.innerHeight - navEl.clientHeight;
            if(newVal !== this.$store.state.screenHeight){
                this.$store.commit('setScreenHeight', newVal);
            }
        }
    },
    components: {
        PageHome,
        SearchBar,
        SlideMenu
    }
};
</script>

<style lang="scss">
@import "globals.scss";

#main-layout {
  display: flex;
  flex-flow: column nowrap;
  padding-top: 50px;
}

#print-header {
  font-size: xx-large;
  margin-left: 10px;
  display: inline-block;
  border-bottom: solid;
}

#the-nav {
  position: fixed;
  top: 0;
  z-index: 5;
  width: 100%;
  height: 50px;
  display: flex;
  flex-flow: row nowrap;
  justify-content: space-between;
  align-items: center;
  background-color: #2c3e50;
}

.nav-section {
  padding-left: 0.5em;
  display: flex;
  flex-flow: row nowrap;
  align-items: center;
  justify-content: space-around;
}

#text-logo, #text-logo > h1 {
    color: white;
    text-decoration: none;
    font-size: 1.25em;
}

#text-logo:hover{
    text-decoration: none;
}

#nav-search {
  flex-shrink: 1;
  max-width: 400px;
}

.nav-section > * {
  margin-right: 1em;
}

.nav-link {
  text-decoration: none;
  color: white;
  font-weight: normal;
}

.nav-link:hover {
  color: lightgray;
}

#report-title {
  margin-left: 2em;
  margin-bottom: 0;
  font-weight: bold;
  font-size: x-large;
}

#page-title {
  color: white;
  text-shadow: black 2px 2px;
  font-size: x-large;
  border-top: solid;
  border-bottom: solid;
  border-width: 1px;
}

#site-container {
  position: relative;
}

@media (max-width: 720px) {
  #the-logo {
    display: none;
  }
}
@media (max-width: 1200px) {
  #the-logo {
    height: 35px;
  }
  .nav-link {
    font-size: 1em;
  }
}
@media (min-width: 1200px) {
  #the-logo {
    height: 45px;
  }
  .nav-link {
    font-size: 1.25em;
  }
}
</style>