export interface Npu {
  name: string;
  id: string;
  description: string;
  imageUrl: string;
  creativity: {
    score: number;
    votes: number;
  };
  uniqueness: {
    score: number;
    votes: number;
  };
}
