<template>
  <div id="page-home">
    <div id="home-top-row">
      <div style="width: 50%;"></div>
      <div id="what-do-div">
        <h3>
          <span v-if="loggedIn">What would you like to do?</span>
          <span v-else>Log In</span>
        </h3>
      </div>
    </div>
    <div id="home-bottom-row">
      <div id="home-marketing">
        <div class="marketing-row marketing-1">
          <div class="info-div">
            <h2 class="home-market-title">Welcome to some app.</h2>
            <p>This is where some stuff about the app could be written.</p>
          </div>
        </div>
        <div class="marketing-row marketing-2">
          <div class="info-div">
            <h2 class="home-market-title">More stuff!</h2>
            <p>Even more could be written here! Untold troves of information!</p>
          </div>
        </div>
        <div class="marketing-row marketing-3">
          <div class="info-div">
            <h2 class="home-market-title">Random Messages from the Server(s)</h2>
            <p v-for="serverMessage in serverMessages" :key="serverMessage.key" v-text="serverMessage">
            </p>
          </div>
        </div>
      </div>
      <app-login v-if="!loggedIn" class="the-login"></app-login>
      <app-directory v-else class="the-directory"></app-directory>
    </div>
  </div>
</template>

<script>
import AppLogin from "../shared-components/app-login.vue";
import AppDirectory from "./app-directory.vue";

import SetupPage from "../js/SetupPage.js";

import Updates from "../js/Updates.js";

export default {
  data() {
    return {
      serverMessages: [],
    };
  },
  mounted() {
    SetupPage.title("").fluid();
    Updates.register(this, ["random-number"]);
  },
  destroyed(){
      Updates.remove(this);
  },
  computed: {
    loggedIn() {
      return this.$store.state.loggedIn;
    }
  },
  methods: {
    processUpdate : function(group, cmd){
      if(group === "random-number"){
        if(this.serverMessages.length > 10){
          this.serverMessages.shift();
        }
        this.serverMessages = this.serverMessages.concat([cmd]);
      }
    },
  },
  components: {
    AppLogin,
    AppDirectory
  }
};
</script>

<style scoped>
#page-home {
  display: flex;
  flex-flow: column nowrap;
  height: 100%;
}

#home-top-row {
  display: flex;
  flex-flow: row nowrap;
  border-bottom: solid;
}

#what-do-div {
  flex-grow: 1;
  display: flex;
  flex-flow: row nowrap;
  justify-content: center;
  align-items: flex-end;
}

#home-bottom-row {
  display: flex;
  flex-flow: row nowrap;
  flex: 1 1 0;
  min-height: 0;
}

#home-marketing {
  display: flex;
  flex-flow: column nowrap;
  justify-content: space-around;
  border-right: solid;
  width: 50%;
}

.marketing-row {
  display: flex;
  flex-flow: row nowrap;
  overflow: hidden;
}

.marketing-row > * {
  width: 50%;
  flex-grow: 1;
}

.marketing-1 {
  border-bottom: solid;
  align-items: flex-end;
}

.marketing-2 {
  border-bottom: solid;
}

.marketing-3 {
  flex-grow: 1;
}

.info-div {
  display: flex;
  flex-grow: 1;
  flex-flow: column nowrap;
  align-items: center;
  padding: 1em;
}

.img-div {
  width: 50%;
}

.home-market-title {
  color: black;
  font-size: x-large;
  text-align: center;
  text-shadow: #aaa 2px 2px 2px;
}

.the-directory {
  overflow: auto;
  flex-grow: 1;
}

.the-login {
  overflow: auto;
  flex-grow: 1;
}

@media (max-width: 768px) {
  #home-top-row {
    display: none;
  }
  #home-marketing {
    display: none;
  }
}

@media (max-width: 1200px) {
  .img-div {
    display: none;
  }
}

@media (max-height: 800px) {
  .img-div {
    display: none;
  }
}
</style>