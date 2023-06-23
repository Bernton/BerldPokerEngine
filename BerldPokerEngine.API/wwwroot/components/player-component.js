import { ref } from "vue";

export default {
  props: {
    index: {
      type: Number,
      default: 0
    },
    allPlayerEquity: {
      type: Number,
      default: 0
    },
    highestPlayerEquity: {
      type: Number,
      default: 0
    },
    totalEquity: {
      type: Number,
      default: 0
    },
    equities: {
      type: Object,
      default: null
    },
    winEquities: {
      type: Object,
      default: null
    },
    tieEquities: {
      type: Object,
      default: null
    },
    negativeEquities: {
      type: Object,
      default: null
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
  mounted() {
    // TODO: fix update with mounting
    const xValues = [
      "High card",
      "Pair",
      "Two Pair",
      "Three of a kind",
      "Straight",
      "Flush",
      "Full house",
      "Four of a kind",
      "Straight flush",
      "Royal flush"
    ];

    const yValues = this.equities.map((c) => (c / this.allPlayerEquity) * 100);

    var barColors = [
      "rgba(0,0,255,1.0)",
      "rgba(0,0,255,0.9)",
      "rgba(0,0,255,0.8)",
      "rgba(0,0,255,0.7)",
      "rgba(0,0,255,0.6)",
      "rgba(0,0,255,0.5)",
      "rgba(0,0,255,0.4)",
      "rgba(0,0,255,0.3)",
      "rgba(0,0,255,0.2)",
      "rgba(0,0,255,0.1)"
    ];

    new Chart("equityChart" + String(this.index), {
      type: "bar",
      data: {
        labels: xValues,
        datasets: [
          {
            backgroundColor: barColors,
            data: yValues
          }
        ]
      },
      options: {
        legend: { display: false },
        title: {
          display: true,
          text: "Equities"
        },
        scales: {
          yAxes: [
            {
              display: true,
              ticks: {
                max:
                  Math.floor(
                    (this.highestPlayerEquity / this.allPlayerEquity) * 100
                  ) + 1
              }
            }
          ]
        }
      }
    });
  },
  template: `<div>
    <p>Player {{index + 1}}</p>
    <p>Equity: {{ toPercentage(totalEquity, allPlayerEquity)}}</p>

    <canvas v-bind:id="'equityChart' + String(index)" style="flex: 1; height: 18em"></canvas>

    </div>`
};
