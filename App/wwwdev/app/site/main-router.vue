<template>
    <router-view></router-view>
</template>

<script>
export default {
    data() {
        return {
        };
    },
}
</script>

<style>
.label-sm {
    font-size: small;
}
.label-md {
    font-size: normal;
}
.label-lg {
    font-size: large;
    text-shadow: black 1px 1px 1px;
}

input[type=checkbox] {
  transform: scale(2);
  -ms-transform: scale(2); /* IE */
  -moz-transform: scale(2); /* FF */
  -webkit-transform: scale(2); /* Safari and Chrome */
  -o-transform: scale(2); /* Opera */
}
</style>