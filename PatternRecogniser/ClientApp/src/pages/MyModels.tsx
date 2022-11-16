import {Typography, Form, Card, Space, Select, Tooltip, UploadProps } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col, message, Upload } from "antd";
import { useState } from "react";
import { QuestionCircleOutlined } from '@ant-design/icons';

const { Title } = Typography;

const MyModelsPage = () => {
    const [form] = Form.useForm();

    return (
        <div>

            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "76vh" }}>
                        <Row justify="space-around" align="middle">
                            <Title>Rozpoznawanie znaku</Title>
                        </Row>

                        Treść
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default MyModelsPage;