<template>
    <div>
        <div class="row" style="margin-top:15px;">
            <div class="col-sm-1">
            </div>
            <div class="col-sm-10">
                <div class="form-group">
                    <label>
                        Username:
                    </label>
                    <input  v-model="username" v-on:keyup="submitLoginIfEnter" name="username" class="form-control" type="text"/>
                </div>
                <div class="form-group">
                    <label>
                        Password:
                    </label>
                    <input v-model="password" v-on:keyup="submitLoginIfEnter" name="password" class="form-control" type="password"/>
                </div>
                <div v-if="shouldShowLoginMessage">
                    <p v-text="loginMessage" class="failure-message"></p>
                </div>
                <div class="form-group">
                    <button v-on:click="submitLogin" value="Log In" class="btn btn-primary log-in-button">
                        Log In
                    </button>
                </div>
            </div>
        </div>
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
.log-in-button {
  width: 100%;
}
.failure-message {
  color: red;
}
</style>