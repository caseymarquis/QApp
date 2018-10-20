<template>
    <list-editor :items="users" :show-search2="true">
        <div class="col-lg-4 col-md-6" slot-scope="{ item, filter }">
            <app-user  v-model="item" :prop-filter="filter" :allow-edit="false"></app-user>
        </div>
    </list-editor>
</template>

<script>
import api from "../js/api.js";
import SetupPage from "../js/SetupPage.js";
import ListEditor from "../shared-components/list-editor.vue";
import AppUser from "../page-users/app-user.vue";

export default {
    mounted() {
        SetupPage.title("User Profile");
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
            api.get(`User/${this.$store.state.user.id}`).then(user => {
                this.users = [user];
            });
        },
    },
    components: {
        ListEditor, EntUser
    }
};
</script>

<style scoped>
</style>