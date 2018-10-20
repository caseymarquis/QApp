<template>
    <div class="the-row">
        <div class="progress p-outer">
            <div class="bar-text">
                <slot></slot><span v-text="theText"></span>
            </div>
            <div class="progress-bar progress-bar-striped" :class="colorClass" role="progressbar" :style="theStyle">
            </div>
            <div v-if="extra" class="progress-bar progress-bar-striped progress-bar-primary" role="progressbar" :style="extraStyle" :no-text="true">
            </div>
        </div>
    </div>
</template>

<script>

export default {
    props: ['height', 'amount', 'max', 'extra', "success", "info", "noText"],
    mounted() {
    },
    created() {
    },
    data() {
        return {
        };
    },
    computed: {
        colorClass() {
            return {
                'progress-bar-success' : this.success,
                'progress-bar-info' : this.info,
            }
        },
        actualMax(){
            return Math.max(this.amount + this.extra, this.max);
        },
        percent(){
            return Math.round(this.amount*100/this.actualMax);
        },
        extraPercent(){
            return Math.round(this.extra*100/this.actualMax);
        },
        theStyle(){
            return `width: ${this.percent}%;`;
        },
        extraStyle(){
            return `width: ${this.extraPercent}%;`;
        },
        theText(){
            if(this.noText){
                return "";
            }
            if(this.extra && this.extra > 0){
                return `${this.amount} + ${this.extra}`;
            }
            return this.amount;
        }
    },
    methods: {
    },
    components: {}
};
</script>

<style scoped>

.bar-text{
    display: flex;
    position: absolute;
    align-items: center;
    justify-content: center;
    text-align: center;
    font-size: large;
    color: white;
    text-shadow: black 1px 1px 1px;
    z-index: 100;
    width: 100%;
    height: 100%;
}

.p-outer{
    height: 100%;
    margin-bottom: 0;
    position: relative;
    background-color: #8a8a8a;
    border: solid;
    border-width: 1px 1px 1px 1px;
}

.the-row{
    height: 50%;
}
</style>