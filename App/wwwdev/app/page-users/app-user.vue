<template>
    <display-edit :name="internalValue.name" :state="state" :allow-edit="allowEdit"
        v-on:edit="onEdit" v-on:save="onSave" v-on:discard="onDiscard">
        <display-prop v-model="internalValue.name" title="Name" :state="state" :filter="propFilter">
            The display name of the user.
        </display-prop>
        <display-prop v-model="internalValue.email" title="Email" :state="state" :filter="propFilter">
            This is the username/email which allows the user to log in.
        </display-prop>
        <display-prop v-model="internalValue.newSystemPassword" title="System Password" state="awaitEdit" :filter="propFilter" type="password"
            v-on:input="onPasswordSetSystem"> 
            This is the user's password for logging in to the main system (where you are right now).
        </display-prop>
        <display-prop v-model="internalValue.enabled" title="Enabled" :state="state" :filter="propFilter" type="bool">
            If this is unchecked, the user will be unable to log in to the system in any capacity.
        </display-prop>
        <display-prop v-model="internalValue.isAdmin" title="Admin" :state="state" :filter="propFilter" type="bool">
            If this is checked, then the user will have adminstrative access to the system. 
        </display-prop>
        <display-prop v-model="internalValue.id" title="System ID" state="awaitEdit" :filter="propFilter"> 
            This is the system ID of this user within the App.
        </display-prop>

    </display-edit>
</template>

<script>
import EditAndPutBase from "../base-components/edit-and-put-base.vue";

import DisplayEdit from "../shared-components/display-edit.vue";
import DisplayProp from "../shared-components/display-prop.vue";

import AuthState from "../js/AuthState.js";
import api from "../js/api.js";

export default {
    extends: EditAndPutBase,
    props: ["allowEdit"],
    data() {
        return {
            url: "User",
            newTabletPassword: "",
            newSystemPassword: "",
        };
    },
    methods: {
        onPasswordSetSystem(newPassword) {
            let email = this.internalValue.email;
            AuthState.hashPassword(email, newPassword).then((hashedPassword) => {
                api.put(`Auth/SetPassword/${this.internalValue.id}?password=${hashedPassword}`);
            }).catch((ex) => {
                alert('Failed to reset password: ' + ex.message);
            });
        }
    },
    components: {
        DisplayProp,
        DisplayEdit,
    }
};
</script>

<style scoped>
</style>