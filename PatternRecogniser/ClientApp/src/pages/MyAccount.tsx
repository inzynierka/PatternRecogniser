import 'antd/dist/antd.min.css';

import { Button, Card, Col, Row, Typography } from 'antd';

import useWindowDimensions from '../UseWindowDimensions';

const { Title } = Typography;


const MyAccountPage = () => {
    const isOrientationVertical  = useWindowDimensions();

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{paddingBottom: "100px"}}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Moje konto</Title>
                        </Row>

                        <Row justify="space-around" align="middle">
                            <Card bordered={true} style={{width: isOrientationVertical ? "30vw" : "50vw", boxShadow: '0 3px 10px rgb(0 0 0 / 0.2)' }}>
                                <Row justify="center" align="middle">
                                    <Col>
                                        <Row justify="space-around" align="middle" style={{marginRight: "35px"}}>
                                            <p style={{fontSize: "1.5em"}}>Adres e-mail:</p>
                                        </Row>
                                        <Row justify="space-around" align="middle" style={{marginRight: "35px"}}>
                                            <p style={{fontSize: "1.5em"}}>Login:</p>
                                        </Row>
                                    </Col>
                                    <Col>
                                        <Row justify="space-around" align="middle">
                                            <p style={{fontSize: "1.5em"}}>{localStorage.getItem('email')}</p>
                                        </Row>
                                        <Row justify="space-around" align="middle">
                                            <p style={{fontSize: "1.5em"}}>{localStorage.getItem('userId')}</p>
                                        </Row>
                                    </Col>
                                </Row>

                                <Row justify="end" align="middle" style={{marginTop: "20px"}}>
                                    <Button>Zmień hasło</Button>
                                </Row>
                            </Card>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default MyAccountPage;