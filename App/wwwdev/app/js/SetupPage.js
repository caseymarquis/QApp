import store from "./store.js";

let SetupPage = {
    //Used in the main site, should be called by every page in the main site.
    title(newTitle) {
        store.commit("setupSearch", { show: false });
        store.commit("setupSlide");
        store.commit("setPageTitle", newTitle);
        return SetupPage;
    },
    search(vm, placeholder, text){
        let search = vm.$route.query.search;
        if(!search){
            search = text;
        }
        store.commit("setupSearch", {
            show: true,
            text: search,
            placeholder: placeholder,
        });
        return SetupPage;
    },
}

export default SetupPage;