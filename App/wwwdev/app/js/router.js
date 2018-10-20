
let router = {
    appendQuery(vm, newQueryObject){
        vm.$router.replace({query: Object.assign({}, vm.$route.query, newQueryObject) });
    },
}

export default router;