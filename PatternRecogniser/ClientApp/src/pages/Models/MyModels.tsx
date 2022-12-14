import 'antd/dist/antd.min.css';

import { Button, Card, Col, Row, Typography } from 'antd';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { ModelType } from '../../types/ModelType';
import { Urls } from '../../types/Urls';
import { GetModels } from '../Common/GetModels';
import { Loading } from '../Common/Loading';
import { NoData } from '../Common/NoData';
import { SearchBar } from '../Common/SearchBar';
import ModelListElement from './ModelListElement';

const { Title } = Typography;

const MyModelsPage = () => {
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

    const fetchModels = () => {
        setLoading(true);
        let models = GetModels();
        if(models.length > 0) setDataLoaded(true);
        else setDataLoaded(false);
        setModels(models);
        setLoading(false);
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
                                        loading ? 
                                        <Loading />
                                        :
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