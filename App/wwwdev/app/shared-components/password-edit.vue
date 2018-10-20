<template>
    <div>
        <div class="col-sm-1">
        </div>
        <div class="col-sm-10">
            <div class="row">
                <label>
                    New Password:
                </label>
            </div>
            <div class="row">
                <input type="password" class="form-control" v-model="password1"/>
            </div>
            <div class="row">
                <label>
                        Confirm Password:
                </label>
            </div>
            <div class="row">
                <input type="password" class="form-control" v-model="password2"/>
            </div>
            <div class="row" v-if="!passwordsMatch">
                <p v-text="message" class="message" v-bind:class="{ noMatch: !passwordsMatch}"></p>
            </div>
            <div class="row" style="margin-top: 3px;">
                <button class="btn btn-sm btn-info" v-on:click="$emit('cancel')">Cancel</button>
                <button v-if="showConfirm" class="btn btn-sm btn-success" style="margin-left: 3px;" v-on:click="tryConfirm">Confirm</button>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["allowEmptyPassword"],
    data() {
        return {
            password1: "",
            password2: "",
        }
    },
    methods: {
        tryConfirm() {
            if (this.password1 === this.password2) {
                this.$emit('confirm', this.password1);
            }
        }
    },
    computed: {
        passwordsMatch() {
            return this.password1 === this.password2;
        },
        formEmpty() {
            return this.password1 === "" && this.password2 == "";
        },
        message() {
            return (this.passwordsMatch ?
                "Passwords match." : "Passwords do not match.");
        },
        showConfirm() {
            let p1 = this.password1;
            let p2 = this.password2;
            if (this.allowEmptyPassword === true) {
                return (p1 === p2);
            }
            else {
                return (p1 === p2 && p1 !== "");
            }
        },
    }
}
</script>

<style scoped>
.noMatch {
  color: red;
}

.the-well {
  padding: 3px 0 3px 0;
}

.message {
  margin-left: 6px;
}
</style>