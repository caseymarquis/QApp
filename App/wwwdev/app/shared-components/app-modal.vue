<template>
  <transition name="modal">
    <div class="modal-mask">
      <div class="modal-wrapper">
        <div class="modal-container" :style="containerStyle">
          <div class="modal-header">
              <h2 v-text="title">
              </h2>
          </div>

          <div class="modal-body">
            <slot></slot>
          </div>

          <div class="modal-footer">
            <button v-if='showCancel' class="modal-default-button btn btn-info" style="margin-left: 10px;" @click="$emit('close', false)">Cancel</button>
            <button class="modal-default-button btn btn-success" @click="$emit('close', true)">OK</button>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
export default {
    props: ['title', 'showCancel', 'width'],
    data(){
        return {
        };
    },
    computed: {
      containerStyle(){
        return `width: ${this.width || '500px'}`
      }
    },
    methods: {
    }
}
</script>

<style scoped>
.modal-mask {
  position: fixed;
  z-index: 100000;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: table;
  transition: opacity 0.3s ease;
}

.modal-wrapper {
  display: table-cell;
  vertical-align: middle;
}

.modal-container {
  margin: 0px auto;
  padding: 20px 30px;
  background-color: #fff;
  border-radius: 2px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
  transition: all 0.3s ease;
  font-family: Helvetica, Arial, sans-serif;
}

.modal-header h3 {
  margin-top: 0;
  color: #42b983;
}

.modal-header{
  padding-top: 0;
  padding-bottom: 0;
}

.modal-body {
  margin: 20px 0;
}

.modal-default-button {
  float: right;
}

/*
 * The following styles are auto-applied to elements with
 * transition="modal" when their visibility is toggled
 * by Vue.js.
 *
 * You can easily play with the modal transition by editing
 * these styles.
 */

.modal-enter {
  opacity: 0;
}

.modal-leave-active {
  opacity: 0;
}

.modal-enter .modal-container,
.modal-leave-active .modal-container {
  -webkit-transform: scale(1.1);
  transform: scale(1.1);
}
</style>