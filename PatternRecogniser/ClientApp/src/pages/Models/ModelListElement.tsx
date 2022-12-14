import 'antd/dist/antd.min.css';

import { DeleteOutlined } from '@ant-design/icons';
import { Button, Card, Col, message, Row, Typography } from 'antd';

import { ModelType } from '../../types/ModelType';
import { ApiService } from '../../generated/ApiService';
import React from 'react';

const { Title } = Typography;

interface Props {
    model: ModelType
    addingToList?: boolean
    deleteModel?: Function
    listName?: string
}

const ModelListElement = (props: Props) => {
    const displaySymbolNumber = (num : number)  => {
        if(num === 1) return num.toString() + " symbol"
        if(num % 10 >= 2 && num % 10 <= 4) return num.toString() + " symbole"
        return num.toString() + " symboli"
    }

    const detailsHandler = () => {
        console.log("Details for", props.model.name);
    }
    const deleteModelHandle = () => {
        if(props.deleteModel !== undefined )
            props.deleteModel(props.model.name);
    }
    const addToListHandle = () => {
        let apiService = new ApiService();

        apiService.addModelTrainingExperiment(props.listName, props.model.extendedModelId)
        .then((response) => {
            message.success("Dodano model do listy");
            console.log(response);
        })
        .catch((error) => {
            error.then(
                (value: string) => {
                    message.error("Nie udało się dodać modelu do listy");
                    console.error("Coudln't add list ("+props.listName+"):", value, props.model.extendedModelId);
                }
            )
            
        });
    }
    return (
        <div key={"div_" + props.model.name}>
            <Card>
                <Row justify="space-between" align="middle">
                    <Col>
                        <Title level={3}>{props.model.name}</Title>
                        {displaySymbolNumber(props.model.patternNum)}
                    </Col>
                    {
                        (props.addingToList === undefined || props.addingToList === false) ?
                            <Col>
                                <Row justify="end">
                                    <Button 
                                        data-testid="details-button" 
                                        type="primary" 
                                        style={{width: "100px", marginBottom: '10px'}} 
                                        size="large" 
                                        key={"detailsButton_" + props.model.name}
                                        onClick={detailsHandler}
                                    >
                                        Szczegóły
                                    </Button>
                                </Row>
                                <Row justify="end">
                                    <Button 
                                        data-testid="delete-button" 
                                        type="default" 
                                        shape="circle" 
                                        icon={<DeleteOutlined />} 
                                        key={"deleteButton" + props.model.name} 
                                        size="large" 
                                        onClick={deleteModelHandle}
                                    />
                                </Row>
                            </Col>
                            :
                            <Col>
                                <Row justify="end">
                                    <Button 
                                        data-testid="add-to-list-button" 
                                        type="primary" 
                                        style={{width: "150px", marginBottom: '10px'}} 
                                        size="large" 
                                        key={"addButton_" + props.model.name}
                                        onClick={addToListHandle}
                                    >
                                        Dodaj do listy
                                    </Button>
                                </Row>
                            </Col>
                    }
                </Row>
                
            </Card>
        </div>
    );
}

export default ModelListElement;