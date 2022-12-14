import 'antd/dist/antd.min.css';

import { Button, Card, Col, message, Row, Typography } from 'antd';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ApiService } from '../../generated/ApiService';
import { ModelType } from '../../types/ModelType';
import { Urls } from '../../types/Urls';
import { Loading } from '../Common/Loading';
import { NoData } from '../Common/NoData';
import { SearchBar } from '../Common/SearchBar';
import ModelListElement from './ModelListElement';

const { Title } = Typography;

const MyModelsPage = () => {
    const apiService = new ApiService();
    const navigate = useNavigate();
    const [models, setModels] = useState<ModelType[]>([]);
    const [displayedModels, setDisplayedModels] = useState<ModelType[]>([]);
    const [loading, setLoading] = useState(false);
    const [dataLoaded, setDataLoaded] = useState(false);
    const [deletedModel, setDeletedModel] = useState(false);
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        setDisplayedModels(models.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }
    const addNewModelHandler = () => {
        navigate(Urls.Train, {replace: true});
    }
    const deleteModelHandler = (modelName : string) => {
        apiService.deleteModel(modelName)
        .then(response => message.success("Pomyślnie usunięto model " + modelName))
        .catch(error => message.error("Nie udało się usunąć modelu"))
        setDisplayedModels(displayedModels.filter(item => item.name !== modelName))
        setModels(models.filter(item => item.name !== modelName))
        setDeletedModel(true);
    }

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
    const fetchModels = () => {
        setLoading(true);
        apiService.getModels()
            .then(response => response.json())
            .then(
                (data) => {
                    if(data !== undefined) {
                        const models = parseModelData(data);

                        setModels(models);
                        setDisplayedModels(models);
                        setDataLoaded(true);
                    }
                    else setDataLoaded(false);
                    setLoading(false);
                }
            )
        setDeletedModel(false);
        return;
    }
    useEffect(() => {
        fetchModels();
    }, [setDeletedModel])

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "75vh" }}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Moje modele</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <SearchBar 
                                onPressEnterHandler={filter} 
                                button={<Button data-testid="add-model-button" type="default" onClick={addNewModelHandler}>Dodaj nowy model</Button>}
                            />

                            <Row justify="space-around" align="middle">
                                <Card data-testid="model-list-card" bordered={true} style={{width: "80vw"}}>
                                    {
                                        loading ? 
                                        <Loading />
                                        :
                                        displayedModels.length > 0 && dataLoaded ?
                                            displayedModels.map((item: ModelType) => (<ModelListElement model={item} key={item.name} deleteModel={deleteModelHandler}/> ))
                                            :
                                            <NoData />
                                    }
                                </Card>
                            </Row>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default MyModelsPage;