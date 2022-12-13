import 'antd/dist/antd.min.css';

import { QuestionCircleOutlined, SearchOutlined, InboxOutlined } from '@ant-design/icons';
import { Button, Card, Col, Input, Row, Space, Tooltip, Typography } from 'antd';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import { ModelType } from '../../types/ModelType';
import { Urls } from '../../types/Urls';
import ModelListElement from './ModelListElement';
import { ApiService } from '../../generated/ApiService';
import { NoData } from '../NoData';
import { SearchBar } from '../SearchBar';

const { Title } = Typography;

const MyModelsPage = () => {
    const apiService = new ApiService();
    const navigate = useNavigate();
    const [models, setModels] = useState<ModelType[]>([]);
    const [displayedModels, setDisplayedModels] = useState<ModelType[]>([]);
    const [loading, setLoading] = useState(false);
    const [dataLoaded, setDataLoaded] = useState(false);
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        setDisplayedModels(models.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }

    const addNewModelHandler = () => {
        navigate(Urls.Train, {replace: true});
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
        let token = localStorage.getItem('token') || "";
        apiService.getModels(token)
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
                },
                (error) => {
                    console.error(error);
                }
            )
        return;
    }

    useEffect(() => {
        fetchModels();
    }, [])

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
                                        displayedModels.length > 0 && dataLoaded ?
                                            displayedModels.map((item: ModelType) => (<ModelListElement model={item} key={item.name}/> ))
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