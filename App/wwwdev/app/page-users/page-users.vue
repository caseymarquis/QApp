<template>
    <list-editor :items="users" :show-create-new="true" :show-search2="true" v-on:createNew="onCreateNew">
        <div class="col-lg-4 col-md-6" slot-scope="{ item, filter }">
            <app-user  v-model="item" :prop-filter="filter"></app-user>
        </div>
    </list-editor>
</template>

<script>
import api from "../js/api.js";
import SetupPage from "../js/SetupPage.js";
import ListEditor from "../shared-components/list-editor.vue";
import AppUser from "./app-user.vue";

export default {
    mounted() {
        SetupPage.title("Configure Users").search(this);
    },
    created() {
        this.fetchData();
    },
    data() {
        return {
            users: [],
        };
    },
    methods: {
        fetchData() {
            api.get('User').then(users => {
                this.users = users;
            });
        },
        onCreateNew() {
            if (confirm("Are you sure you want to create a new user?")) {
                api.post('User').then((user) => {
                    this.fetchData();
                });
            }
        }
    },
    components: {
        ListEditor, AppUser
    }
};
</script>

<style scoped>
</style>