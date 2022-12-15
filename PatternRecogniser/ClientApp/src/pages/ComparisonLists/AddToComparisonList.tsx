import 'antd/dist/antd.min.css';

import { ArrowLeftOutlined, QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { Button, Card, Col, Input, message, Row, Space, Tooltip, Typography } from 'antd';
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

import { ApiService } from '../../generated/ApiService';
import { ExperimentType } from '../../types/ExperimentListType';
import { ModelType } from '../../types/ModelType';
import { Urls } from '../../types/Urls';
import ModelListElement from '../Models/ModelListElement';
import { CreateComparisonListModal } from './CreateComparisonListModal';
import { NoData } from '../Common/NoData';
import { Loading } from '../Common/Loading';

const { Title } = Typography;

interface Props {
    creating : boolean
}

const AddToComparisonListPage = (props : Props) => {
    const params = useParams();
    const apiService = new ApiService();
    const navigate = useNavigate();
    const [models, setModels] = useState<ModelType[]>([]);
    const [displayedModels, setDisplayedModels] = useState<ModelType[]>([]);
    const [loading, setLoading] = useState(false);
    const [dataLoaded, setDataLoaded] = useState(false);
    const [, setDeletedModel] = useState(false);
    const [listName, setListName] = useState(params.listName as string || "");
    
    const filter = (e : any) => {
        let searchName = e.target.defaultValue
        console.log(searchName)
        setDisplayedModels(models.filter(item => item.name.toLowerCase().includes(searchName.toLowerCase())))
    }
    const goBackHandler = () => {
        navigate(Urls.ComparisonLists, {replace: true});
    }

    const modalOkHandler = (name : string) => {
        setListName(name);
        console.log(name);
        apiService.createExperimentList(name, ExperimentType.Models)
        .then((response) => {
            message.success("Utworzono listę");
            console.log(response);
        })
        .catch((error) => {
            message.error("Nie udało się utworzyć listy");
            console.log("Coudln't create a list: ", error);
        });
    }
    const modalCancelHandler = () => {
        setListName("");
        navigate(Urls.ComparisonLists, {replace: true});
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
                        <Button onClick={goBackHandler} icon={<ArrowLeftOutlined style={{fontSize: '2em'}}/>} size="large" shape="circle" type="text" style={{marginLeft: '20px', marginTop: '15px'}}/>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px", marginTop: "-55px"}}>
                            <Title>Dodaj do listy</Title>
                        </Row>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px", marginTop: "-45px"}}>
                            <Title level={3}>{listName}</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Row justify="space-between" align="middle" style={{width: "80vw", marginBottom: '20px'}}>
                                <p></p>
                                <Col>
                                    <Space>
                                        <Input placeholder="Wyszukaj" prefix={<SearchOutlined />} onPressEnter={filter}/>
                                        <Tooltip title="Wciśnij ENTER aby wyszukać modelu po nazwie.">
                                            <Typography.Link><QuestionCircleOutlined /></Typography.Link>
                                        </Tooltip>
                                    </Space>
                                </Col>
                            </Row>

                            <Row justify="space-around" align="middle">
                                <Card bordered={true} style={{width: "80vw"}}>
                                    {
                                         loading ? 
                                         <Loading />
                                         :
                                         displayedModels.length > 0 && dataLoaded ?
                                         displayedModels.map((item: ModelType) => (<ModelListElement model={item} addingToList={true} listName={listName} key={item.name}/> ))
                                             :
                                             <NoData />
                                    }
                                </Card>
                            </Row>
                        </Row>
                    </div>      
                </Col>
            </Row>
            {
                props.creating &&
                <CreateComparisonListModal modalOpen={listName.length === 0} onOkHandler={modalOkHandler} onCancelHandler={modalCancelHandler} />    
            }   
        </div>
    );
}

export default AddToComparisonListPage;