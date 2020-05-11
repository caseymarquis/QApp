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

<style no-scope>
label {
  font-weight: bold;
  margin-bottom: 0;
  font-size: 1.1em;
}

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

input[type="checkbox"] {
  -ms-transform: scale(2); /* IE */
  -moz-transform: scale(2); /* FF */
  -webkit-transform: scale(2); /* Safari and Chrome */
  -o-transform: scale(2); /* Opera */
  transform: scale(2); /* Normal */
  cursor: pointer;
}

input[type="date"],
input[type="datetime-local"],
input[type="email"],
input[type="month"],
input[type="number"],
input[type="password"],
input[type="search"],
input[type="tel"],
input[type="text"],
input[type="time"],
input[type="url"],
input[type="week"],
input[type="datetime"],
input[type=""],
select
{
  border-width: 2px;
  -webkit-box-shadow: none;
  box-shadow: none;
  display: block;
  width: 100%;
  height: 45px;
  padding: 10px 15px;
  font-size: 15px;
  line-height: 1.42857143;
  color: #2c3e50;
  background-color: #fff;
  background-image: none;
  border: 1px solid #dce4ec;
  border-radius: 4px;
  -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);
  box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);
  -webkit-transition: border-color ease-in-out 0.15s,
    box-shadow ease-in-out 0.15s;
  -o-transition: border-color ease-in-out 0.15s, box-shadow ease-in-out 0.15s;
  transition: border-color ease-in-out 0.15s, box-shadow ease-in-out 0.15s;
}

h2 {
  font-size: large;
  margin-top: 10.5px;
}

a {
  color: #001dff;
  cursor: pointer;
  font-weight: bold;
}

.primary {
  background-color: #2c3e50;
  border-color: #2c3e50;
  color: #fff;
  text-shadow: black 1px 1px 1px;
  font-size: larger;
}

.info {
  background-color: #3498db;
  border-color: #3498db;
  color: #fff;
  text-shadow: black 1px 1px 1px;
  font-size: larger;
}

.btn-black {
  background-color: black;
  color: white;
}

.btn-black:hover {
  background-color: #222;
}

html {
  -webkit-print-color-adjust: exact !important;
}

.panel-warning-light > .panel-heading {
  color: #000;
  background-color: #ffe3b6;
  border-color: #f39c12;
}

.panel-success-light > .panel-heading {
  color: #000;
  background-color: #c1eae2;
  border-color: #18bc9c;
}

@media (max-height: 600px) {
  * {
    font-size: 13px !important;
  }
}

@media (max-width: 600px) {
  * {
    font-size: 13px !important;
  }
}

@media print {
  .no-print {
    display: none !important;
  }

  table {
    /*Fixes tables when printing.*/
    font-size: 1vw;
  }
}

@media screen {
  .only-print {
    display: none !important;
  }
}
</style>