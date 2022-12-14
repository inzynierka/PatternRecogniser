import 'antd/dist/antd.min.css';

import { Card, Col, message, Row, Typography } from 'antd';
import { useEffect, useState } from 'react';

import { ApiService } from '../../../generated/ApiService';
import { TrainModelMessages } from '../../../components/BackendMessages';
import useWindowDimensions from '../../../UseWindowDimensions';
import { TrainForm } from './TrainForm';
import Training from './Training';

const { Title } = Typography;


const TrainPage = () => {
    const apiService = new ApiService();
    const isOrientationVertical  = useWindowDimensions();
    const [isModelBeingTrained, setIsModelBeingTrained] = useState(false);

    const getUpdate = () => {
        apiService.getModelStatus("")
        .then((status) => {
            if(status === TrainModelMessages.modelIsTrained || status === TrainModelMessages.modelIsInQueue) {
                setIsModelBeingTrained(true);
            }
            else if (status === TrainModelMessages.modelTrainingComplete) {
                setIsModelBeingTrained(false);
                message.success('Model został wytrenowany pomyślnie.');
            }
            else {
                setIsModelBeingTrained(false);
                if(status === TrainModelMessages.modelTrainingFailed) message.error('Nie udało się wytrenować modelu.');
            }
          });
    };
    useEffect(() => {
        getUpdate();
    }, [])

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{paddingBottom: "79px"}}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "25px"}}>
                            <Title>Trenowanie modelu</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Card bordered={true} style={{width: isOrientationVertical ? "40vw" : "65vw", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)', paddingTop: '35px' }}>
                                {
                                    isModelBeingTrained ?
                                    <Training />
                                    :
                                    // Formularz do trenowania modelu
                                    <TrainForm setIsModelBeingTrained={setIsModelBeingTrained}/>
                                }
                            </Card>
                        </Row>
                    </div> 
                    <br />            
                </Col>
            </Row>
        </div>
    );
}

export default TrainPage;