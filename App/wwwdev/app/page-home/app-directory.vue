<template>
    <div>
        <ul style="list-style:none; margin-top: 7px;" class="directory">
            <li>
                <h3 class="homeDirTitle">Do Some Stuff</h3>
                <ul class="homeSubList">
                    <li>
                        <router-link to="/some-page" class="homePageLink">
                            Some Page
                        </router-link>
                    </li>
                </ul>
            </li>
            <li v-if="isAdmin">
                <h3 class="homeDirTitle">Admin Only</h3>
                <ul class="homeSubList">
                    <li>
                        <router-link to="/users" class="homePageLink">
                            Configure Users
                        </router-link>
                    </li>
                </ul>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
    data() {
        return {};
    },
    computed: {
        isAdmin() {
            return this.$store.state.user.isAdmin;
        }
    }
}
</script>

<style scoped>
.title-p {
  font-weight: bold;
}
ul.directory li,
ul,
li,
p {
  /*css is the worst...*/
  margin-bottom: 2px;
}

ul.directory li,
h3 {
  /*css is the worst...*/
  margin-top: 2px;
  margin-bottom: 2px;
}
</style>
