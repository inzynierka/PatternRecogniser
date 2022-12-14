import 'antd/dist/antd.min.css';

import { ApiService } from '../../generated/ApiService';
import { ModelType } from '../../types/ModelType';

const parseModelData = (data : any) => {
    let models : ModelType[] = [];
    let model : ModelType;

    data.forEach((item: any) => {
        model = {
            name: item.name,
            patternNum: item.patterns === null ? 0 : item.patterns.length(),
            distribution: item.distribution,
            extendedModelId: item.extendedModelId,
        }
        models.push(model);
    })

    return models;
}

export const GetModels = () => {
    const apiService = new ApiService();
    let models : ModelType[] = [];

    let token = localStorage.getItem('token') || "";
    apiService.getModels(token)
        .then(response => response.json())
        .then(
            (data) => {
                if(data !== undefined) {
                    models = parseModelData(data);
                }
            }
        )

    return models;
}