import { ref } from "vue";
import { evaluate } from "../services/evalutionApi.js";
import {
  storeEvaluationInput,
  getStoredEvaluationInput
} from "../services/evaluationInputStorage.js";
import EvaluationOutputComponent from "./evaluation-output-component.js";

const defaultInititalEvaluationInput = "XxXxXxXxXx AcTc 3d3c"; // "XxXxXxXxXx XxXx";

export default {
  components: {
    EvaluationOutputComponent
  },
  setup() {
    const isEvaluationLoaderVisible = ref(false);
    const inititalEvaluationInput =
      getStoredEvaluationInput() ?? defaultInititalEvaluationInput;
    const evaluationInput = ref(inititalEvaluationInput);
    const errorMessage = ref(String());
    const evaluationData = ref({});

    return {
      evaluationInput,
      isEvaluationLoaderVisible,
      evaluationData,
      errorMessage,
      evaluate: async function () {
        const response = await evaluate(evaluationInput.value);
        const data = await response.json();

        if (response.ok) {
          storeEvaluationInput(evaluationInput.value);
          const extendedData = extendEvaluationData(data);
          console.log(extendedData);
          evaluationData.value = extendedData;
        } else {
          errorMessage.value = data;
        }
      },
      wrapLoader: async function (action) {
        isEvaluationLoaderVisible.value = true;
        await action();
        isEvaluationLoaderVisible.value = false;
      }
    };
  },
  template: `<h1>BerldPokerEngine.Web</h1>
    <div
      style="
        display: flex;
        width: 100%;
        height: 3em;
        align-items: center;
        gap: 1em;
      "
    >
      <input
        type="text"
        v-model="evaluationInput"
        style="width: 17.5em; height: 1.3em"
      />
      <input
        type="button"
        value="Evaluate"
        style="height: 1.8em"
        @click="() => wrapLoader(evaluate)"
      />
      <div v-if="isEvaluationLoaderVisible" class="loader"></div>
    </div>
    <p v-if="errorMessage.length > 0" style="color: red"> {{ errorMessage }}</p>

    <evaluation-output-component v-bind="evaluationData"></evaluation-output-component>`
};

function extendEvaluationData(data) {
  let extendedData = { ...data };
  let totalEquity = 0;
  let highestPlayerEquity = 0;

  data.playerStats.forEach((player, playerI) => {
    let totalPlayerEquity = 0;
    let equities = [];

    for (let i = 0; i < 10; i++) {
      const equity = player.winEquities[i] + player.tieEquities[i];

      if (equity > highestPlayerEquity) {
        highestPlayerEquity = equity;
      }

      totalPlayerEquity += equity;
      equities.push(equity);
    }

    totalEquity += totalPlayerEquity;
    extendedData.playerStats[playerI].totalEquity = totalPlayerEquity;
    extendedData.playerStats[playerI].equities = equities;
  });

  extendedData.totalEquity = totalEquity;
  extendedData.highestPlayerEquity = highestPlayerEquity;
  return extendedData;
}
