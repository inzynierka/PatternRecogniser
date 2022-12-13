import { IExperiment } from "../generated/ApiService";

export enum ExperimentType {
    PatternRecognition = "PatternRecognitionExperiment",
    ModelTraining = "ModelTrainingExperiment"
}

export interface ComparisonListType {
    experimentListId: number,
    name: string,
    elementNum: number,
    experimentType: ExperimentType,

    usedModel?: string,
    experiments?: IExperiment[]
}