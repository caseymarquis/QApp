<template>
    <div></div>
</template>

<script>
import api from "../js/api.js";

export default {
  props: ["value", "propFilter"],
  data() {
    return {
      state: "awaitEdit",
      editing: false,
      internalValue: null,
    };
  },
  created(){
      this.resetData();
  },
  watch: {
    "value" : function(){
      this.resetData();
    }
  },
  methods: {
    resetData(){
      this.internalValue = JSON.parse(JSON.stringify(this.value));
    },
    onEdit() {
      this.state = "edit";
    },
    onSave() {
      this.state = "save";
      api
        .put(`${this.url}/${this.internalValue.id}`, this.internalValue)
        .then(acceptedData => {
          this.state = "awaitEdit";
          this.internalValue = JSON.parse(JSON.stringify(acceptedData));
        })
        .catch(msg => {
          this.state = "edit";
          alert(msg);
        });
    },
    onDiscard() {
      this.state = "awaitEdit";
      this.resetData();
    }
  }
};
</script>