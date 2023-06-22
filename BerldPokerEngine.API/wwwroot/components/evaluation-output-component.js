export default {
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
    }
  },
  setup() {
    return {
      toPercentage(value, total) {
        const percent = (value / total) * 100;
        return percent.toFixed(2) + "%";
      }
    };
  },
  template: `<div v-if="playerStats">
    <h2>Elapsed: {{ timeInMilliseconds }} ms</h2>
    <h2>Equity: {{ totalEquity }}</h2>

    <li v-for="player in playerStats">
    Equity Player {{player.index + 1}}: {{ toPercentage(player.totalEquity, totalEquity)}}
    </li>
  </div>`
};
