<template>
  <div class="component" :class="{'wide-search': wide}">
    <input :id="id" class="search-input" :class="{'bold-placeholder': boldPlaceholder}" type="text" autocomplete="off" :placeholder="placeholder" v-model="searchText" v-on:keyup="raiseSearch" v-on:blur="onBlur" v-on:focus="onFocus" @click="onClick" />
    <button class="btn btn-secondary search-btn" v-on:click="raiseSearch">
      <fa-icon icon="search" size="lg" />
    </button>
  </div>
</template>

<script>
import router from "../js/router.js";

let searchIdSuffix = 0;

export default {
  props: ["value", "placeholder", "wide", "focusOnMounted", "boldPlaceholder", "syncToUrl", "urlId"],
  data() {
    searchIdSuffix++;
    return {
      id: `app-search-input-${searchIdSuffix}`,
      searchText: "",
      searchSequence: 0,
    }
  },
  created() {
    if(this.syncToUrl){
      this.setFromQuery();
    }
    else{
      this.searchText = (this.value || "");
    }
  },
  mounted() {
    if (this.focusOnMounted) {
      let el = document.getElementById(this.id);
      if (el) {
        el.focus();
      }
    }
  },
  watch: {
    'value': 'updateInternalValue'
  },
  computed: {
    actualUrlId(){
      return this.urlId || "searchbar"
    }
  },
  methods: {
    raiseSearch() {
      this.searchSequence++;
      let searchSequenceWas = this.searchSequence;
      setTimeout(() => {
        if (this.searchSequence === searchSequenceWas) {
          this.$emit("search", this.searchText);
          this.$emit("input", this.searchText);
          this.setQuery(this.searchText);
        }
      }, 400);
    },
    onClick() {
      this.$emit('click', this.searchText);
    },
    onBlur() {
      this.$emit('blur', this.searchText);
    },
    onFocus() {
      this.$emit('focus', this.searchText);
    },
    updateInternalValue() {
      this.setQuery(this.value);
      this.searchText = this.value;
    },
    setQuery(value){
      if(this.syncToUrl){
        let obj = {};
        obj[this.actualUrlId] = value;
        router.appendQuery(this, obj);
      }
    },
    setFromQuery(){
      if(this.syncToUrl){
        this.searchText = this.$route.query[this.actualUrlId];
        this.raiseSearch();
      }
    },
  }
}
</script>

<style scoped>
.component {
  display: flex;
  flex-flow: row nowrap;
}

.wide-search {
  flex-grow: 1;
}

.search-input {
    flex-grow: 1;
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
}

.search-btn {
    border-top-left-radius: 0;
    border-bottom-left-radius: 0;
}

.bold-placeholder::placeholder {
  font-weight: bold;
}
.bold-placeholder::-webkit-input-placeholder {
  font-weight: bold;
}
.bold-placeholder::-moz-placeholder {
  font-weight: bold;
}
.bold-placeholder::-ms-placeholder {
  font-weight: bold;
}
</style>