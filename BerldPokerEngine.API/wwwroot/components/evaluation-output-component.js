import PlayerComponent from "./player-component.js";

export default {
  components: {
    PlayerComponent
  },
  props: {
    isExhaustive: {
      type: Boolean,
      default: false
    },
    timeInMilliseconds: {
      type: Number,
      default: 0
    },
    playerStats: {
      type: Object,
      default: null
    },
    totalEquity: {
      type: Number,
      default: 0
    },
    highestPlayerEquity: {
      type: Number,
      default: 0
    }
  },
  setup() {},
  template: `<div v-if="playerStats">
    <p>Elapsed: {{ timeInMilliseconds }} ms</p>
    <p>Equity: {{ totalEquity }}</p>

    <div style="display: flex; flex-wrap: wrap">
      <template v-for="player in playerStats">
        <player-component v-bind="player" v-bind:highestPlayerEquity="highestPlayerEquity" v-bind:allPlayerEquity="totalEquity"></player-component>
      </template>
    </div>
  </div>`
};
