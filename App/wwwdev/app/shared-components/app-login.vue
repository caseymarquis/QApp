<template>
  <div class="app-login">
    <label>Email:</label>
    <input v-model="username" v-on:keyup="submitLoginIfEnter" name="username" type="text" />
    <label>Password:</label>
    <input v-model="password" v-on:keyup="submitLoginIfEnter" name="password" type="password" />
    <p v-if="shouldShowLoginMessage" v-text="loginMessage"></p>
    <button v-on:click="submitLogin" value="Log In" class="btn btn-primary log-in-button">Log In</button>
  </div>
</template>

<script>
import AuthState from "../js/AuthState.js";

export default {
    props: [],
    data() {
        return {
            username: "",
            password: "",
        };
    },
    computed: {
        shouldShowLoginMessage() {
            let msg = this.$store.state.loginMessage;
            if (msg !== undefined && msg !== null && msg.length > 0) {
                return true;
            }
            return false;
        },
        loginMessage() {
            return this.$store.state.loginMessage;
        }
    },
    methods: {
        submitLogin() {
            let needUser = (this.username === undefined || this.username === null || this.username === "");
            let needPassword = (this.password === undefined || this.password === null || this.password === "");
            if (needUser) {
                this.$store.commit('loginFailed', "Enter a user name.");
            }
            else if(needPassword){
                this.$store.commit('loginFailed', "Enter a password.");
            }
            else {
                AuthState.updateAuthToken(this.username, this.password, this.$store);
            }
        },
        submitLoginIfEnter(ev) {
            if (ev.keyCode === 13) {
                this.submitLogin();
            }
        }
    },
    components: {
    }
}
</script>

<style>
.app-login {
  display: flex;
  flex-flow: column nowrap;
  align-items: center;
  padding: 1em 5% 0 5%;
}

.app-login > label,
.app-login > p {
  align-self: flex-start;
}

.app-login > p {
  color: red;
}

.app-login > input {
  margin-bottom: 1em;
}

.app-login > button {
  width: 100%;
}
</style>