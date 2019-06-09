<template>
    <div class='acd'>
        <input id='user' tabindex="-1" type="email"/>
        <input id='username' tabindex="-1" type="text"/>
        <input id='password' type="password" tabindex="-1"/>
    </div>
</template>

<script>
export default {
    data() {
        return {
        }
    },
}
</script>

<style scoped>
    .acd {
        position: absolute;
        left: -2000%;
    }
</style>