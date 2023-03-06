export interface Service {
  id: number | null;
  name: string | null;
}
export interface IntegrationAPI {
  id: number | null;
  nodeID: number | null;
  nodeName: string | null;
  ServiceList: string | null;
  service: Service[];
}
export interface EnvironmentModel {
  id: number;
  name: string | null;
  comment: string | null;
  listIntegrationAPI: IntegrationAPI[];
}
export interface NodeSetingsModel {
  listEnvironments: EnvironmentModel[];
}
export interface ApiResponse {
  isSuccessed: boolean;
  message: string;
}
